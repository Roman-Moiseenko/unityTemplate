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

        private readonly Coroutines _coroutines;

        public ReactiveProperty<float> TimeOutNewWaveValue = new(0f);
        
        public ReactiveProperty<bool> FinishWave = new(false); //Волна закончилась
        public ReactiveProperty<bool> StartWave = new(false); //Волна началась


        private readonly ObservableList<MobViewModel> _allMobsOnWay = new();
        public IObservableCollection<MobViewModel> AllMobsOnWay => _allMobsOnWay;


        public GateWaveViewModel GateWaveViewModel;
        public GateWaveViewModel GateWaveSecondViewModel; //TODO Сделать, когда будет 2 пути
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
            
            _fsmWave = container.Resolve<FsmWave>();
            _wayService = container.Resolve<WayService>();
            var roadsService = container.Resolve<RoadsService>();
            //Подписка на новую волну, при изменении номера волны, запускаем корутин старт волны
            gameplayState.CurrentWave.Skip(1)
                .Subscribe(newValue =>
                    {
                        if (newValue > gameplayState.CountWaves) return;
                        //Создаем мобов для всей волны
                        var command = new CommandCreateWave{ Index = newValue};
                        _cmd.Process(command);
                        _coroutines.StartCoroutine(StartNewWave());
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
                    freeRoad = false; //На дороге есть мобы, она не свободна
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
            roadsService.Way.ObserveAdd().Subscribe(_ => MoveGateWaveViewModel());
            //Если 2 пути
            if (_gameplayState.HasWaySecond.Value)
            {
                CreateGateWaveSecondViewModel();    
                roadsService.WaySecond.ObserveAdd().Subscribe(_ => MoveGateWaveSecondViewModel());
            }
            
            //При добавлении дороги на путь, перемещаем модель ворот
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

        private IEnumerator StartNewWave()
        {
            yield return new WaitUntil(() => _fsmWave.IsBegin()); //Ждем когда разрешиться запуск волны
            StartWave.OnNext(true);
            yield return new WaitForSeconds(0.8f); //Пауза
            _fsmWave.Fsm.SetState<FsmStateWaveGo>();
            yield return GenerateMob(); //Выводим мобов на дорогу
            yield return new WaitForSeconds(0.5f);

            if (_gameplayState.CountWaves != _gameplayState.CurrentWave.Value) // && !_gameplayState.IsInfinity()
            {
                _fsmWave.Fsm.SetState<FsmStateWaveWait>();
                _gameplayState.CurrentWave.Value++;
            }
        }

        private IEnumerator GenerateMob()
        {
            foreach (var mobEntity in _gameplayState.BufferMobs.ToList())
            {
                _gameplayState.Mobs.Add(mobEntity);
                _gameplayState.BufferMobs.Remove(mobEntity);
                yield return new WaitForSeconds(SpeedGenerateMobs);
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
            //_gameplayState.GateWave.OnNext(position);

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
            _gameplayState.GateWave = GateWaveViewModel.Position;
        }
        
            private void CreateGateWaveSecondViewModel()
            {
                var lastPoint = _wayService.GetLastPoint(_gameplayState.Origin.WaySecond);
                var exitPoint = _wayService.GetExitPoint(_gameplayState.Origin.WaySecond);
    
                Vector2 position = new Vector2((lastPoint.x + exitPoint.x) / 2f, (lastPoint.y + exitPoint.y) / 2f);
                var direction = exitPoint - lastPoint;
                
                //_gameplayState.GateWaveSecond.OnNext(position);
    
                GateWaveSecondViewModel = new GateWaveViewModel(_fsmWave)
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
                _gameplayState.GateWaveSecond = GateWaveSecondViewModel.Position;
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
        private void MoveGateWaveSecondViewModel()
        {
            var lastPoint = _wayService.GetLastPoint(_gameplayState.Origin.WaySecond);
            var exitPoint = _wayService.GetExitPoint(_gameplayState.Origin.WaySecond);
            Vector2 position = (exitPoint + lastPoint) / 2;
            var direction = exitPoint - lastPoint;
            GateWaveSecondViewModel.Position.Value = position;
            GateWaveSecondViewModel.Direction.Value = direction;
        }

        private void CreateMobViewModel(MobEntity mobEntity)
        {
            mobEntity.IsWay = true;
            var mobViewModel = new MobViewModel(mobEntity, _gameplayState, this);
            _allMobsOnWay.Add(mobViewModel);
        }

        public List<RoadPoint> GenerateRoadPoints(MobEntity mobEntity)
        {
            var way = mobEntity.IsWay ? _gameplayState.Origin.Way : _gameplayState.Origin.WaySecond;
            List<RoadPoint> roads = new();
//            Debug.Log(mobViewModel.UniqueId);

            //Формируем список точек движения моба
            for (int i = way.Count - 1; i >= 0; i--)
            {
                Vector2 position = way[i].Position; //Определяем новые координаты моба
                //Смещение от центра на delta
                if (way[i].IsTurn)
                {
                    position.x += mobEntity.Delta;
                    position.y += mobEntity.Delta;
                }
                else
                {
                    if (way[i].Rotate % 2 == 0)
                    {
                        position.y += mobEntity.Delta;
                    }
                    else
                    {
                        position.x += mobEntity.Delta;
                    }
                }
                
                var direction = ( i == 0 ? Vector2Int.zero : way[i - 1].Position) - way[i].Position;
                roads.Add(new RoadPoint(position, direction)); //Список точек движения 
            }
            roads.Add(new RoadPoint(Vector2.zero, Vector2Int.left));
            
            return roads;
        }
    }
}