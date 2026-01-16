using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.WaveStates;
using Game.GamePlay.View.Mobs;
using Game.GamePlay.View.Waves;
using Game.State.Maps.Mobs;
using Game.State.Maps.Roads;
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
        private const float TimeEndWave = 0.8f; //Задержка между волнами
        
        private readonly GameplayStateProxy _gameplayState;
        private readonly WayService _wayService;
        private readonly FsmGameplay _fsmGameplay;
        private readonly Coroutines _coroutines;
        
        public ReactiveProperty<float> TimeOutNewWaveValue = new(0f);
        
        public ReactiveProperty<bool> IsMobsOnWay = new(); //Мобы на дороге
        public ReactiveProperty<bool> FinishWave = new(false); //Волна закончилась
        public ReactiveProperty<bool> StartWave = new(false); //Волна началась
        public ReactiveProperty<bool> FinishAllWaves = new(false); //Волны закончились

        private readonly ObservableList<MobViewModel> _allMobsOnWay = new();
        public IObservableCollection<MobViewModel> AllMobsOnWay => _allMobsOnWay;
        public readonly ObservableDictionary<int, MobEntity> AllMobsMap = new();

        public GateWaveViewModel GateWaveViewModel;
        public GateWaveViewModel GateWaveViewModelSecond; //TODO Сделать, когда будет 2 пути
        private readonly GameplayCamera _cameraService;
        private Coroutine _coroutineTimerNewWave;
        private readonly FsmWave _fsmWave;

        public WaveService(
            DIContainer container,
            GameplayStateProxy gameplayState
        )
        {
            _gameplayState = gameplayState;
            _coroutines = GameObject.Find("[COROUTINES]").GetComponent<Coroutines>();
            _fsmGameplay = container.Resolve<FsmGameplay>();
            _fsmWave = container.Resolve<FsmWave>();
            _wayService = container.Resolve<WayService>();
            var roadsService = container.Resolve<RoadsService>();
            _cameraService = container.Resolve<GameplayCamera>();
            //Подписка на новую волну, при изменении номера волны, запускаем корутин старт волны
            gameplayState.CurrentWave
                .Skip(1)
                .Where(newValue => newValue <= gameplayState.Waves.Count && newValue != 0)
                .Subscribe(newValue => _coroutines.StartCoroutine(StartNewWave(newValue))
            );

            _allMobsOnWay.ObserveAdd().Subscribe(newValue =>
            {
                if (!IsMobsOnWay.CurrentValue) //На дороге есть мобы
                {
                    IsMobsOnWay.Value = true;
                    _coroutines.StopCoroutine(_coroutineTimerNewWave);
                }

                var trigger = _allMobsOnWay.Any(viewModel =>
                    viewModel.NumberWave == newValue.Value.NumberWave &&
                    viewModel.MobEntityId != newValue.Value.MobEntityId
                );
                if (!trigger) StartWave.OnNext(true); //Это первый моб из волны = > Показать надпись Волна идет

                var mobViewModel = newValue.Value;
                _coroutines.StartCoroutine(
                    MovingMobOnWay(newValue.Value)); //При добавлении моба, запускаем его движение

                mobViewModel.Debuffs.ObserveAdd().Subscribe(d =>
                    _coroutines.StartCoroutine(MobTimerDebuff(d.Value.Key, d.Value.Value, mobViewModel)));

                mobViewModel.Debuffs.ObserveRemove().Subscribe(d =>
                    _coroutines.StopCoroutine(MobTimerDebuff(d.Value.Key, d.Value.Value, mobViewModel)));
            });

            _allMobsOnWay.ObserveRemove().Subscribe(e =>
            {
                _coroutines.StopCoroutine(MovingMobOnWay(e.Value)); //Прерываем движение моба
                _gameplayState.KillMobs.Value++;
                var trigger = _allMobsOnWay.Any(mobViewModel => mobViewModel.NumberWave == e.Value.NumberWave);
                if (!trigger) FinishWave.OnNext(true); //Текущая волна моба закончилась
                
                if (_allMobsOnWay.Count != 0) return;
                _fsmWave.Fsm.SetState<FsmStateWaveTimer>();
                IsMobsOnWay.Value = false;
                
            });
            
            AllMobsMap.ObserveRemove().Subscribe(e =>
            {
                _coroutines.StartCoroutine(
                    RemoveMobViewModel(e.Value
                        .Key)); //Сущность удалена с дороги, запускаем корутину удаления модели
            });
            //Создаем модель ворот
            CreateGateWaveViewModel();
            //При добавлении дороги на путь, перемещаем модель ворот
            roadsService.Way.ObserveAdd().Subscribe(_ => MoveGateWaveViewModel());
            roadsService.WaySecond.ObserveAdd().Subscribe(_ => MoveGateWaveViewModelSecond());

            _fsmWave.Fsm.StateCurrent.Subscribe(v =>
            {
                if (v.GetType() == typeof(FsmStateWaveGo))
                {
                    _coroutines.StopCoroutine(_coroutineTimerNewWave);
                    TimeOutNewWaveValue.Value = 0;
                }
                if (v.GetType() == typeof(FsmStateWaveWait) && _allMobsOnWay.Count == 0)
                {
                    _fsmWave.Fsm.SetState<FsmStateWaveTimer>();
                }
                if (v.GetType() == typeof(FsmStateWaveTimer))
                {
                    TimeOutNewWaveValue.Value = 0;
                    _coroutineTimerNewWave = _coroutines.StartCoroutine(TimerNewWave());
                }
            });
        }

        public void Start()
        {
            _fsmWave.Fsm.SetState<FsmStateWaveTimer>(); //Первоначальный запуска таймера
        }
        public void StartNextWave()
        {
            _gameplayState.CurrentWave.Value++;
        }
        public void StartForcedNewWave()
        {
            _fsmWave.Fsm.SetState<FsmStateWaveBegin>();
        }

        private IEnumerator StartNewWave(int numberWave)
        {
            yield return new WaitUntil(() => _fsmWave.IsBegin()); //Ждем когда разрешиться запуск волны
            while (_fsmGameplay.IsPause()) yield return null;
            yield return new WaitForSeconds(0.8f); //Пауза
            _fsmWave.Fsm.SetState<FsmStateWaveGo>();
            yield return GenerateMob(numberWave); //Выводим мобов на дорогу
            yield return new WaitForSeconds(0.5f); 
            _fsmWave.Fsm.SetState<FsmStateWaveWait>();
            
            if (_gameplayState.Waves.Count == _gameplayState.CurrentWave.Value && !_gameplayState.IsInfinity())
            {
                FinishAllWaves.OnNext(true);
            }
            else
            {
                _gameplayState.CurrentWave.Value++;    
            }
        }

        private IEnumerator GenerateMob(int numberWave)
        {
            if (!_gameplayState.Waves.TryGetValue(numberWave, out var waveEntity)) yield break; //Волны закончились
            foreach (var entityMob in waveEntity.Mobs)
            {
                while (_fsmGameplay.IsPause()) yield return null;
                //Задержка создания нового моба
                CreateMobViewModel(entityMob, waveEntity.Number);
                yield return new WaitForSeconds(SpeedGenerateMobs); // / GameSpeed.CurrentValue
            }
            _fsmWave.Fsm.SetState<FsmStateWaveEnd>(); //Все мобы вышли
        }

        private IEnumerator MovingMobOnWay(MobViewModel mobViewModel)
        {
            yield return _fsmGameplay.WaitPause();
            yield return mobViewModel.MovingModel(GenerateRoadPoints(mobViewModel));
            yield return mobViewModel.AttackEntity(_gameplayState.Castle);
        }

        public IEnumerator MobTimerDebuff(string configId, MobDebuff debuff, MobViewModel mobViewModel)
        {
            yield return _fsmGameplay.WaitPause();
            yield return new WaitForSeconds(debuff.Time);
            mobViewModel.RemoveDebuff(configId);
        }

        private IEnumerator TimerNewWave()
        {
            for (var i = 0; i <= AppConstants.TIME_WAVE_NEW; i++) //Ускоряем при новой скорости
            {
                if (!_fsmWave.IsTimer()) yield break; //Если во время цикла перешли в др.состояние
                TimeOutNewWaveValue.Value = Convert.ToSingle(i) / AppConstants.TIME_WAVE_NEW;
                yield return new WaitForSeconds(AppConstants.TIME_PAUSE_WAVE_NEW);
            }
            _fsmWave.Fsm.SetState<FsmStateWaveBegin>();
        }

        private IEnumerator RemoveMobViewModel(int mobId)
        {
            var mobViewModel = _allMobsOnWay.FirstOrDefault(e => e.MobEntityId == mobId);
            if (mobViewModel == null) yield break;
            mobViewModel.StartAnimationDelete(); //Запускаем процесс удаления модели
            yield return mobViewModel.WaitFinishAnimation(); //Ждем удаления модели
            _allMobsOnWay.Remove(mobViewModel);
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

            GateWaveViewModel = new GateWaveViewModel(_fsmWave)
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

        private void CreateMobViewModel(MobEntity mobEntity, int numberWave)
        {
            //TODO Загружаем параметры моба из настроек и передаем их в созданую модель
            var position = GateWaveViewModel.Position.Value;
            var direction = -1 * GateWaveViewModel.Direction.Value;
            mobEntity.SetStartPosition(position, direction);

            mobEntity.IsWay = true;
            var mobViewModel = new MobViewModel(mobEntity, this, _cameraService, _fsmGameplay);
            mobViewModel.NumberWave = numberWave;
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