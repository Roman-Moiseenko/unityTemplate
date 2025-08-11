using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Fsm;
using Game.GamePlay.View.Mobs;
using Game.GamePlay.View.Waves;
using Game.State.Maps.Mobs;
using Game.State.Maps.Roads;
using Game.State.Maps.Waves;
using Game.State.Root;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.GamePlay.Services
{
    public class WaveService
    {
        private const float SpeedGenerateMobs = 0.5f; //Скорость генерации новых мобов
        private const float TimeEndWave = 1f; //Задержка между волнами
        //private const int TimeNewWave = 50; //Кол-во задержек по 0.1сек перед новой волной

        private readonly DIContainer _container;
        private readonly GameplayStateProxy _gameplayState;
        private readonly WayService _wayService;
        private readonly FsmGameplay _fsmGameplay;
        private readonly Coroutines _coroutines;
        private readonly RoadsService _roadsService;

        public readonly ReactiveProperty<int> GameSpeed;

        public ReactiveProperty<bool>
            StartForced = new(false); //Комбинация различных подписок для разрешения запуска новой волны

        public ReactiveProperty<bool> TimeOutNewWave = new(false); //Таймер ожидания волны закончился
        public ReactiveProperty<float> TimeOutNewWaveValue = new(0f);
        public ReactiveProperty<bool> ShowGate = new(false); //Для подписки внешним ViewModel


        //public ObservableList<MobEntity> CurrentWave = new(); //Список мобов на дороге
        public ReactiveProperty<bool> IsMobsOnWay = new(); //Мобы на дороге 
        private readonly ObservableList<MobViewModel> _allMobsOnWay = new();
        public IObservableCollection<MobViewModel> AllMobsOnWay => _allMobsOnWay;
        public readonly ObservableDictionary<int, MobEntity> AllMobsMap = new();

        public GateWaveViewModel GateWaveViewModel;
        public GateWaveViewModel GateWaveViewModelSecond; //TODO Сделать, когда будет 2 пути
        private readonly GameplayCamera _cameraService;

        public WaveService(
            DIContainer container,
            GameplayStateProxy gameplayState
        )
        {
            _container = container;
            _gameplayState = gameplayState;
            _coroutines = GameObject.Find("[COROUTINES]").GetComponent<Coroutines>();
            _fsmGameplay = container.Resolve<FsmGameplay>();
            _wayService = container.Resolve<WayService>();
            _roadsService = container.Resolve<RoadsService>();
            _cameraService = container.Resolve<GameplayCamera>();
            GameSpeed = gameplayState.GameSpeed;
            //Debug.Log(JsonConvert.SerializeObject(gameplayState.Origin.Waves, Formatting.Indented));

            //Комбинированная подписка, с одним результатом => Запустить процесс создания мобов на новой волне 
            Observable.Merge(
                _fsmGameplay.IsGamePause,
                IsMobsOnWay,
                TimeOutNewWave
            ).Skip(1).Subscribe(newValue =>
            {
                StartForced.Value = !_fsmGameplay.IsGamePause.CurrentValue && !IsMobsOnWay.CurrentValue &&
                                    TimeOutNewWave.CurrentValue;
                // if (!StartForced.Value) Debug.Log("StartForced.Value = " + !_fsmGameplay.IsGamePause.CurrentValue + !IsMobsOnWay.CurrentValue + TimeOutNewWave.CurrentValue);
            });

            //Подписка на новую волну, при изменении номера волны, запускаем корутин старт волны
            gameplayState.CurrentWave.Skip(1).Subscribe(newValue =>
            {
                if (newValue <= gameplayState.Waves.Count && newValue != 0)
                    _coroutines.StartCoroutine(StartNewWave(newValue));
            });

            _allMobsOnWay.ObserveAdd().Subscribe(newValue =>
            {
                if (!IsMobsOnWay.CurrentValue) //На дороге есть мобы
                {
                    IsMobsOnWay.Value = true;
                    _coroutines.StopCoroutine(TimerNewWave());
                }

                var mobViewModel = newValue.Value;
                _coroutines.StartCoroutine(
                    MovingMobOnWay(newValue.Value)); //При добавление моба, запускаем его движение

                mobViewModel.Debuffs.ObserveAdd().Subscribe(d =>
                    _coroutines.StartCoroutine(MobTimerDebuff(d.Value.Key, d.Value.Value, mobViewModel)));
                
                mobViewModel.Debuffs.ObserveRemove().Subscribe(d =>
                    _coroutines.StopCoroutine(MobTimerDebuff(d.Value.Key, d.Value.Value, mobViewModel)));
            });

            _allMobsOnWay.ObserveRemove().Subscribe(e =>
            {
   
                
                if (_allMobsOnWay.Count != 0) return; //На дороге нет мобов
                IsMobsOnWay.Value = false;
                _coroutines.StartCoroutine(TimerNewWave());
            });
            AllMobsMap.ObserveRemove().Subscribe(e =>
            {
                _coroutines.StartCoroutine(
                    RemoveMobViewModel(e.Value.Key)); //Сущность удалена с дороги, запускаем корутину удаления модели
            });
            //Создаем модель ворот
            CreateGateWaveViewModel();
            //При добавлении дороги на путь, перемещаем модель ворот
            _roadsService.Way.ObserveAdd().Subscribe(e => MoveGateWaveViewModel());
            _roadsService.WaySecond.ObserveAdd().Subscribe(e => MoveGateWaveViewModelSecond());
            _coroutines.StartCoroutine(TimerNewWave()); //Первоначальный запуска таймера
        }

        public void StartNextWave()
        {
            _gameplayState.CurrentWave.Value++;
        }

        public void StartForcedNewWave()
        {
            _coroutines.StopCoroutine(TimerNewWave());
            //  TimeOutNewWave.Value = false;
            StartForced.Value = true;
        }

        private IEnumerator StartNewWave(int numberWave)
        {
            yield return new WaitUntil(() => StartForced.CurrentValue); //Ждем когда разрешиться запуск волны
            ShowGate.Value = true; //Показать ворота
            yield return _coroutines.StartCoroutine(GenerateMob(numberWave)); //Выводим мобов на дорогу
            yield return new WaitForSeconds(TimeEndWave); //Пауза между волнами
            ShowGate.Value = false; //Убрать ворота и Показать инфо модель
            //GateWaveViewModel.ShowInfoModel(); 
            _gameplayState.CurrentWave.Value++;
        }

        private IEnumerator GenerateMob(int numberWave)
        {
            if (!_gameplayState.Waves.TryGetValue(numberWave, out var waveEntity)) yield break; //Волны закончились
            foreach (var entityMob in waveEntity.Mobs)
            {
                yield return new WaitUntil(() => !_fsmGameplay.IsGamePause.Value); //Пауза игры
                CreateMobViewModel(entityMob);
                yield return
                    new WaitForSeconds(SpeedGenerateMobs / GameSpeed.CurrentValue); //Задержка создания нового моба
            }
        }

        private IEnumerator MovingMobOnWay(MobViewModel mobViewModel)
        {
            yield return _fsmGameplay.WaitPause();
          /*  while (_fsmGameplay.IsGamePause.Value)//Пауза
            {
                yield return null;
            }*/
            yield return mobViewModel.MovingModel(GenerateRoadPoints(mobViewModel));
            //mobViewModel.State.Value = MobState.Attacking;
            yield return mobViewModel.AttackCastle();
            
            yield return MobAttackCastle(mobViewModel); //Моб дошел до замка Запуск атаки моба
           // yield return DamageToMob(mobViewModel); //Наносим урон мобу --- заменить на урон Замку
            yield break;
        }

        public IEnumerator MobTimerDebuff(string configId, MobDebuff debuff, MobViewModel mobViewModel)
        {
            yield return _fsmGameplay.WaitPause();
            
            yield return new WaitForSeconds(debuff.Time);
            mobViewModel.RemoveDebuff(configId);
        }

        private IEnumerator TimerNewWave()
        {
            TimeOutNewWave.Value = false;
            for (var i = 0; i < AppConstants.TIME_WAVE_NEW; i++) //Ускоряем при новой скорости
            {
                yield return _fsmGameplay.WaitPause();
                /*while (_fsmGameplay.IsGamePause.Value)
                {
                    yield return null;
                }*/

                // yield return new WaitUntil(() => !_fsmGameplay.IsGamePause.Value);
                TimeOutNewWaveValue.Value = Convert.ToSingle(i) / AppConstants.TIME_WAVE_NEW;

                yield return new WaitForSeconds(0.04f / GameSpeed.CurrentValue);
                // Debug.Log("WaitForSeconds = " + 0.04f / GameSpeed.CurrentValue);
            }

            TimeOutNewWave.Value = true;
        }

        private IEnumerator RemoveMobViewModel(int mobId)
        {
            var mobViewModel = _allMobsOnWay.FirstOrDefault(e => e.MobEntityId == mobId);
            if (mobViewModel == null) yield break;

            mobViewModel.StartAnimationDelete(); //Запускаем процесс удаления модели
            /*while (!mobViewModel.FinishCurrentAnimation.CurrentValue)
            {
                yield return null;
            }*/
            yield return mobViewModel.WaitFinishAnimation(); //Ждем удаления модели
            _allMobsOnWay.Remove(mobViewModel);
            yield return null;
        }

        /**
         * Временная функция убийства мобов
         */
        private IEnumerator DamageToMob(MobViewModel mobViewModel)
        {
            try
            {
                AllMobsMap[mobViewModel.MobEntityId].Health.Value -= 1000;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                yield break;
            }

            yield return null;
        }

        private IEnumerator MobAttackCastle(MobViewModel mobViewModel)
        {
            
            if (mobViewModel == null) yield break;
            yield return _fsmGameplay.WaitPause();
            
            _gameplayState.Castle.DamageReceived(mobViewModel.Attack);

            yield return new WaitForSeconds(AppConstants.MOB_BASE_SPEED);
         //   yield return MobAttackCastle(mobViewModel);
        }
        

        /**
         * Создаем модель ворот для главного пути и вычисляем координаты и направление поворота ворот
         */
        private void CreateGateWaveViewModel()
        {
            var lastPoint = _wayService.GetLastPoint(_gameplayState.Origin.Way);
            var exitPoint = _wayService.GetExitPoint(_gameplayState.Origin.Way);

            Vector2 position = new Vector2((lastPoint.x + exitPoint.x) / 2f, (lastPoint.y + exitPoint.y) / 2f);
            var direction = exitPoint - lastPoint;
            
            GateWaveViewModel = new GateWaveViewModel(this)
            {
                Position =
                {
                    Value = position
                },
                Direction =
                {
                    Value = direction
                }
            };
        }

        /**
         * Перемещаем главные ворота
         */
        private void MoveGateWaveViewModel()
        {
            var lastPoint = _wayService.GetLastPoint(_gameplayState.Origin.Way);
            var exitPoint = _wayService.GetExitPoint(_gameplayState.Origin.Way);
            Vector2 position = new Vector2((lastPoint.x + exitPoint.x) / 2f, (lastPoint.y + exitPoint.y) / 2f);
            var direction = exitPoint - lastPoint;
            GateWaveViewModel.Position.Value = position;
            GateWaveViewModel.Direction.Value = direction;
        }

        /**
         * Перемещаем вторые ворота
         */
        private void MoveGateWaveViewModelSecond()
        {
            var lastPoint = _wayService.GetLastPoint(_gameplayState.Origin.WaySecond);
            var exitPoint = _wayService.GetExitPoint(_gameplayState.Origin.WaySecond);
            Vector2 position = (exitPoint + lastPoint) / 2;
            var direction = exitPoint - lastPoint;
            GateWaveViewModelSecond.Position.Value = position;
            GateWaveViewModelSecond.Direction.Value = direction;
        }

        private void CreateMobViewModel(MobEntity mobEntity)
        {
            //TODO Загружаем параметры моба из настроек и передаем их в созданую модель
            var position = GateWaveViewModel.Position.Value;
            var direction = -1 * GateWaveViewModel.Direction.Value;
            mobEntity.SetStartPosition(position, direction);

            mobEntity.IsWay = true;
            var mobViewModel = new MobViewModel(mobEntity, this, _cameraService);
            AllMobsMap.Add(mobEntity.UniqueId, mobEntity);
            _allMobsOnWay.Add(mobViewModel);
        }

        private List<RoadPoint> GenerateRoadPoints(MobViewModel mobViewModel)
        {
            var way = _gameplayState.Origin.Way;
            List<RoadPoint> roads = new();

            //Формируем список точек движения моба
            for (int i = way.Count - 1; i >= 0; i--)
            {
                Vector2 position = way[i].Position; //Определяем новые координаты моба
                //Смещение от центра на delta
                if (way[i].IsTurn)
                {
                    position.x += mobViewModel.Delta;
                    position.y += mobViewModel.Delta;
                }
                else
                {
                    if (way[i].Rotate % 2 == 0)
                    {
                        position.y += mobViewModel.Delta;
                    }
                    else
                    {
                        position.x += mobViewModel.Delta;
                    }
                }

                Vector2Int direction;
                if (i == 0)
                {
                    position.x -= 0.15f;
                    direction = Vector2Int.zero - way[i].Position;
                }
                else
                {
                    direction = way[i - 1].Position - way[i].Position;
                }
                
                //TODO если i = 0, то добавляем 0.25 по направлению
                //TODO для IsFly при i = 1, а при i = 0 нет движения
                roads.Add(new RoadPoint(position, direction)); //Список точек движения 
            }

            return roads;
        }
    }
}