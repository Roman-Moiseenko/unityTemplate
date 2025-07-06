using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.View.Mobs;
using Game.GamePlay.View.Waves;
using Game.State.Maps.Mobs;
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
        private const int TimeNewWave = 50; //Кол-во задержек по 0.1сек перед новой волной

        private readonly DIContainer _container;
        private readonly GameplayStateProxy _gameplayState;
        private readonly WayService _wayService;
        private readonly FsmGameplay _fsmGameplay;
        private readonly Coroutines _coroutines;
        private readonly RoadsService _roadsService;

        public ReactiveProperty<bool>
            StartForced = new(false); //Комбинация различных подписок для разрешения запуска новой волны

        public ReactiveProperty<bool> TimeOutNewWave = new(false); //Таймер ожидания волны закончился


        //public ObservableList<MobEntity> CurrentWave = new(); //Список мобов на дороге
        public ReactiveProperty<bool> IsMobsOnWay = new();
        private readonly ObservableList<MobViewModel> _allMobsOnWay = new();
        public IObservableCollection<MobViewModel> AllMobsOnWay => _allMobsOnWay;
        private readonly Dictionary<int, MobViewModel> _mobsMap = new();

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
            
            //Комбинированная подписка, с одним результатом => Запустить процесс создания мобов на новой волне 
            Observable.Merge(
                _fsmGameplay.IsGamePause,
                IsMobsOnWay,
                TimeOutNewWave
            ).Skip(1).Subscribe(newValue =>
            {
                StartForced.Value = !_fsmGameplay.IsGamePause.CurrentValue && !IsMobsOnWay.CurrentValue &&
                                    TimeOutNewWave.CurrentValue;
            });
            //Debug.Log(JsonConvert.SerializeObject(gameplayState.Waves, Formatting.Indented));

            //Подписка на новую волну, при изменении номера волны, запускаем корутин старт волны
            gameplayState.CurrentWave.Skip(1).Subscribe(newValue =>
            {
                if (newValue <= gameplayState.Waves.Count && newValue != 0)
                    _coroutines.StartCoroutine(StartNewWave(newValue));
            });

            
            gameplayState.CurrentWave.Subscribe();
            gameplayState.Waves.ObserveRemove().Subscribe(e =>
            {
                //TODO Если кол-во оставшихся волн = 0, то победа
            });

            _allMobsOnWay.ObserveAdd().Subscribe(newValue =>
            {
                if (!IsMobsOnWay.CurrentValue) //На дороге есть мобы
                {
                    IsMobsOnWay.Value = true;
                    _coroutines.StopCoroutine(TimerNewWave());
                }
            });
            _allMobsOnWay.ObserveRemove().Subscribe(e =>
            {
                RemoveMobViewModel(e.Value);
                if (_allMobsOnWay.Count == 0) //На дороге нет мобов
                {
                    IsMobsOnWay.Value = false;
                    _coroutines.StartCoroutine(TimerNewWave());
                }
            });
            //Создаем модель ворот
            CreateGateWaveViewModel();
            //При добавлении дороги на путь, перемещаем модель ворот
            _roadsService.Way.ObserveAdd().Subscribe(e => MoveGateWaveViewModel());
            _roadsService.WaySecond.ObserveAdd().Subscribe(e => MoveGateWaveViewModelSecond());
        }

        private IEnumerator StartNewWave(int numberWave)
        {
            Debug.Log("StartNewWave старт Ждем подтверждения запуска");
            yield return new WaitUntil(() => StartForced.CurrentValue); //Ждем когда разрешиться запуск волны
            yield return _coroutines.StartCoroutine(GenerateMob(numberWave)); //Выводим мобов на дорогу
            yield return new WaitForSeconds(TimeEndWave); //Пауза между волнами
            Debug.Log("Новая волна");
            _gameplayState.CurrentWave.Value++;
        }

        private IEnumerator GenerateMob(int numberWave)
        {
            if (_gameplayState.Waves.TryGetValue(numberWave, out var waveEntity))
            {
                foreach (var entityMob in waveEntity.Mobs)
                {
                    yield return new WaitUntil(() => !_fsmGameplay.IsGamePause.Value); //Пауза игры
                    CreateMobViewModel(entityMob);
                    yield return new WaitForSeconds(SpeedGenerateMobs); //Задержка создания нового моба
                }
            }
        }

        private IEnumerator TimerNewWave()
        {
            TimeOutNewWave.Value = false;
            //Разбиваем цикл на доли, всего 5 секунд 
            for (var i = 0; i < TimeNewWave; i++)
            {
                yield return new WaitUntil(() => !_fsmGameplay.IsGamePause.Value); //При паузе не срабатывает
                yield return new WaitForSeconds(0.1f);
                Debug.Log("Timer работает");
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
            //TODO Загружаем параметры моба из настроек и передаем их в созданую модель
            var position = GateWaveViewModel.Position.Value;
            var direction = GateWaveViewModel.Direction.Value;
            mobEntity.Position.Value = new Vector3(position.x, mobEntity.Position.CurrentValue.y, position.y);
            mobEntity.Direction.Value = direction;
            mobEntity.IsWay = true;
            var mobViewModel = new MobViewModel(mobEntity);
            _allMobsOnWay.Add(mobViewModel);
            _mobsMap.Add(mobViewModel.MobEntityId, mobViewModel);
        }

        private void RemoveMobViewModel(MobViewModel mobViewModel)
        {
            _allMobsOnWay.Remove(mobViewModel);
            _mobsMap.Remove(mobViewModel.MobEntityId);
        }

        public void StartNextWave()
        {
            _gameplayState.CurrentWave.Value++;
        }
    }
}