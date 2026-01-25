using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Commands.WaveCommands;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.WaveStates;
using Game.GamePlay.View.Mobs;
using Game.GamePlay.View.Waves;
using Game.State.Maps.Mobs;
using Game.State.Maps.Roads;
using Game.State.Root;
using MVVM.CMD;
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
        private readonly ICommandProcessor _cmd;
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
        //public readonly ObservableDictionary<int, MobEntity> AllMobsMap = new();

        public GateWaveViewModel GateWaveViewModel;
        public GateWaveViewModel GateWaveViewModelSecond; //TODO Сделать, когда будет 2 пути
        private readonly GameplayCamera _cameraService;
        private Coroutine _coroutineTimerNewWave;
        private readonly FsmWave _fsmWave;

        public WaveService(
            DIContainer container,
            GameplayStateProxy gameplayState,
            ICommandProcessor cmd
        )
        {
            _gameplayState = gameplayState;
            _cmd = cmd;
            _coroutines = GameObject.Find("[COROUTINES]").GetComponent<Coroutines>();
            _fsmGameplay = container.Resolve<FsmGameplay>();
            _fsmWave = container.Resolve<FsmWave>();
            _wayService = container.Resolve<WayService>();
            var roadsService = container.Resolve<RoadsService>();
            _cameraService = container.Resolve<GameplayCamera>();
            //Подписка на новую волну, при изменении номера волны, запускаем корутин старт волны
            gameplayState.CurrentWave.Skip(1)
                .Subscribe(newValue =>
                    {
                        if (newValue > gameplayState.CountWaves) return;
                        //Создаем мобов для всей волны
                        var command = new CommandCreateWave{ Index = newValue};
                        _cmd.Process(command);
                        _coroutines.StartCoroutine(StartNewWave(newValue));
                    }
                );
            
            //Подписка на всех мобов из Уровня
            gameplayState.Mobs.ObserveAdd().Subscribe(e =>
            {
                var mobEntity = e.Value;
                CreateMobViewModel(mobEntity);
                mobEntity.Debuffs.ObserveAdd().Subscribe(d =>
                    _coroutines.StartCoroutine(MobTimerDebuff(d.Value.Key, d.Value.Value, mobEntity)));

                mobEntity.Debuffs.ObserveRemove().Subscribe(d =>
                    _coroutines.StopCoroutine(MobTimerDebuff(d.Value.Key, d.Value.Value, mobEntity)));
            });
            
            gameplayState.Mobs.ObserveRemove().Subscribe(e =>
            {
                var mobEntity = e.Value;
                _coroutines.StartCoroutine(RemoveMobViewModel(mobEntity.UniqueId));
                _gameplayState.KillMobs.Value++;

                var finishWave = true;
                var freeRoad = true;
                foreach (var stateMob in gameplayState.Mobs)
                {
                    if (stateMob.IsWentOut.CurrentValue) freeRoad = false;
                    if (stateMob.NumberWave == mobEntity.NumberWave) finishWave = false;
                }
                FinishWave.OnNext(finishWave);
                if (freeRoad)
                {
                    _fsmWave.Fsm.SetState<FsmStateWaveTimer>();
                }
                
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
            StartWave.OnNext(true);
            yield return new WaitForSeconds(0.8f); //Пауза
            _fsmWave.Fsm.SetState<FsmStateWaveGo>();
            yield return GenerateMob(numberWave); //Выводим мобов на дорогу
            yield return new WaitForSeconds(0.5f);

            if (_gameplayState.CountWaves != _gameplayState.CurrentWave.Value) // && !_gameplayState.IsInfinity()
            {
                _fsmWave.Fsm.SetState<FsmStateWaveWait>();
                _gameplayState.CurrentWave.Value++;
            }
        }

        private IEnumerator GenerateMob(int numberWave)
        {
            foreach (var mobEntity in _gameplayState.Mobs.ToList())
            {
                if (mobEntity.NumberWave == numberWave)
                {
                    mobEntity.IsWentOut.OnNext(true);
                    yield return new WaitForSeconds(SpeedGenerateMobs);    
                }
            }
            _fsmWave.Fsm.SetState<FsmStateWaveEnd>(); //Все мобы вышли
        }

        public IEnumerator MobTimerDebuff(string configId, MobDebuff debuff, MobEntity mobEntity)
        {
            yield return new WaitForSeconds(debuff.Time);
            mobEntity.RemoveDebuff(configId);
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
            var mobViewModel = _allMobsOnWay.FirstOrDefault(e => e.UniqueId == mobId);
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

        private void CreateMobViewModel(MobEntity mobEntity)
        {
            mobEntity.IsWay = true;
            var mobViewModel = new MobViewModel(mobEntity, _cameraService, _gameplayState, this);
            _allMobsOnWay.Add(mobViewModel);
        }

        public List<RoadPoint> GenerateRoadPoints(MobEntity mobViewModel)
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
                 //   position.x -= 0.15f;
                    direction = Vector2Int.zero - way[i].Position;
                }
                else
                {
                    direction = way[i - 1].Position - way[i].Position;
                }
                
                roads.Add(new RoadPoint(position, direction)); //Список точек движения 
            }

            //mobViewModel.RoadPoints = roads;
            return roads;
        }
    }
}