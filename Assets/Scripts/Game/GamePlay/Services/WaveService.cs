using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.Common;
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

        public readonly ReactiveProperty<int> GameSpeed = new(1);

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

        public Dictionary<int, WaveEntityData> Waves;

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

            /*gameplayState.GameSpeed.Subscribe(e =>
            {
                if (e != 0)
                {
                    GameSpeed.Value = e;
                }
            });*/
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
                _coroutines.StartCoroutine(
                    MovingMobOnWay(newValue.Value)); //При добавление моба, запускаем его движение
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

        private IEnumerator MovingMobOnWay(MobViewModel mobViewModel)
        {
            //mobViewModel.SetStartPosition(GateWaveViewModel.Position.CurrentValue, GateWaveViewModel.Direction.CurrentValue);
            var way = _gameplayState.Origin.Way;
            List<Vector2> roads = new();
            for (int i = way.Count - 1; i >= 0; i--)
            {
                //Определяем направление кроме начальной позиции
                /*  if (i != way.Count - 1)
                  {
                      var direction = _wayService.GetDirection(way, i);

                      //Запускаем поворот
                      yield return mobViewModel.RotateModel(direction);
                  }*/
                //Определяем новые координаты моба
                Vector2 position = way[i].Position;
                if (i == 0)
                {
                    //TODO проверяем позицию Замка
                    position.x -= 0.25f;
                }

                //TODO если i = 0, то добавляем 0.25 по направлению
                //TODO для IsFly при i = 1, а при i = 0 нет движения
                roads.Add(position); //Список точек движения 
//                Debug.Log("Модель " + mobViewModel.MobEntityId + " Запускаем. Индекс дороги = " + i);

                //Запускаем движение
            }

            yield return mobViewModel.MovingModel(roads);
            //TODO Моб дошел до замка
            yield return DamageToMob(mobViewModel); //Наносим урон мобу --- заменить на урон Замку
            yield break;
        }

        private IEnumerator StartNewWave(int numberWave)
        {
            //   Debug.Log("1 TimeOutNewWave.Value = " + TimeOutNewWave.Value);
            yield return new WaitUntil(() => StartForced.CurrentValue); //Ждем когда разрешиться запуск волны
            //   Debug.Log("2 TimeOutNewWave.Value = " + TimeOutNewWave.Value);
            ShowGate.Value = true; //Показать ворота
            //GateWaveViewModel.ShowGateModel();
            yield return _coroutines.StartCoroutine(GenerateMob(numberWave)); //Выводим мобов на дорогу
            //  Debug.Log("3 TimeOutNewWave.Value = " + TimeOutNewWave.Value);
            yield return new WaitForSeconds(TimeEndWave); //Пауза между волнами
//            Debug.Log("4 TimeOutNewWave.Value = " + TimeOutNewWave.Value);
            ShowGate.Value = false; //Убрать ворота и Показать инфо модель
            //GateWaveViewModel.ShowInfoModel(); 
            _gameplayState.CurrentWave.Value++;
            //         Debug.Log("5 TimeOutNewWave.Value = " + TimeOutNewWave.Value);
        }

        private IEnumerator GenerateMob(int numberWave)
        {
            if (_gameplayState.Waves.TryGetValue(numberWave, out var waveEntity))
            {
                foreach (var entityMob in waveEntity.Mobs)
                {
                    yield return new WaitUntil(() => !_fsmGameplay.IsGamePause.Value); //Пауза игры
                    CreateMobViewModel(entityMob);
                    yield return
                        new WaitForSeconds(SpeedGenerateMobs / GameSpeed.CurrentValue); //Задержка создания нового моба
                }
            }
        }

        private IEnumerator TimerNewWave()
        {
            TimeOutNewWave.Value = false;
            int timeWave = (GameSpeed.CurrentValue == 0) ? AppConstants.TIME_WAVE_NEW : AppConstants.TIME_WAVE_NEW / GameSpeed.CurrentValue;
            //Разбиваем цикл на доли, всего 5 секунд 

            for (var i = 0; i < timeWave; i++) //Ускоряем при новой скорости
            {
                /*while (_fsmGameplay.IsGamePause.Value)
                {
                    yield return null;
                } */
                yield return new WaitUntil(() => !_fsmGameplay.IsGamePause.Value);
                TimeOutNewWaveValue.Value = Convert.ToSingle(i) / timeWave;
                yield return new WaitForSeconds(0.05f);
                
            }

            TimeOutNewWave.Value = true;
        }

        public void WaveCompleted(int numberWave)
        {
            _gameplayState.Waves.Remove(numberWave);
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
            //new Vector2Int(exitPoint.x - lastPoint.x, exitPoint.y - lastPoint.y);
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
            //new Vector2((lastPoint.x + exitPoint.x) / 2f, (lastPoint.y + exitPoint.y) / 2f);
            var direction = exitPoint - lastPoint;
            GateWaveViewModelSecond.Position.Value = position;
            GateWaveViewModelSecond.Direction.Value = direction;
        }

        private void CreateMobViewModel(MobEntity mobEntity)
        {
            //Debug.Log(" GateWaveViewModel.Position.Value = "  + GateWaveViewModel.Position.Value);
            //TODO Загружаем параметры моба из настроек и передаем их в созданую модель
            var position = GateWaveViewModel.Position.Value;
            var direction = GateWaveViewModel.Direction.Value;

            mobEntity.Position.Value = new Vector3(position.x, position.y);
            mobEntity.Direction.Value = direction;
            mobEntity.IsWay = true;
            var mobViewModel = new MobViewModel(mobEntity, this);
            AllMobsMap.Add(mobEntity.UniqueId, mobEntity);
            _allMobsOnWay.Add(mobViewModel);
        }

        private IEnumerator RemoveMobViewModel(int mobId)
        {
            //TODO Запускаем процесс удаления модели
            var mobViewModel = _allMobsOnWay.FirstOrDefault(e => e.MobEntityId == mobId);
            _allMobsOnWay.Remove(mobViewModel);
            //_mobsMap.Remove(mobViewModel.MobEntityId);
            yield return null;
        }

        public void StartNextWave()
        {
            _gameplayState.CurrentWave.Value++;
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

        public void StartForcedNewWave()
        {
            _coroutines.StopCoroutine(TimerNewWave());
            TimeOutNewWave.Value = false;
            StartForced.Value = true;
        }
    }
}