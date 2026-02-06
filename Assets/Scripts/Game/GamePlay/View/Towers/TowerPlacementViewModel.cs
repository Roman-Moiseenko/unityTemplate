using System;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.WaveStates;
using Game.GamePlay.Services;
using Game.GamePlay.View.Mobs;
using Game.GamePlay.View.Warriors;
using Game.State.Maps.Roads;
using Game.State.Maps.Towers;
using Game.State.Maps.Warriors;
using Game.State.Root;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerPlacementViewModel : TowerViewModel
    {
        private readonly PlacementService _placementService;
        public ObservableList<WarriorViewModel> Warriors = new();
        private readonly ObservableList<WarriorEntity> _warriorEntities = new();
        
        public ObservableList<MobViewModel> PullTargets = new();
        public float Range { get; set; }
        public float Health { get; set; }
        public float Damage { get; set; }
        public float Speed { get; set; }
        private readonly List<int> _freeIndex = new();

        public bool IsWay;
        public float DeltaWarrior = 0.15f;

        public Dictionary<int, List<RoadPoint>> AvailablePath = new();
        //Кеш подписок на смерть моба
        private readonly Dictionary<int, IDisposable> _mobDisposables = new(); 
        public ReactiveProperty<Vector2Int> Placement => _towerEntity.Placement;

        public TowerPlacementViewModel(TowerEntity towerEntity, GameplayStateProxy gameplayState,
            TowersService towersService, FsmTower fsmTower, FsmWave fsmWave, PlacementService placementService) : base(
            towerEntity, gameplayState, towersService, fsmTower)
        {
            _placementService = placementService;
            UpdateParameterWarrior();
            for (var i = 1; i <= 3; i++)
                CreateWarriorEntity(i - 2);

            //При изменении точки спавна и обновлении дорог, меняем доступный путь Warriors
            Placement.Subscribe(_ => UpdateWayPath());
            _gameplayState.Way.ObserveAdd().Subscribe(_ => UpdateWayPath());
            _gameplayState.WaySecond.ObserveAdd().Subscribe(_ => UpdateWayPath());

            foreach (var warriorEntity in _warriorEntities)
            {
                var warriorViewModel = new WarriorViewModel(
                    warriorEntity, _gameplayState, _towerEntity, AvailablePath[warriorEntity.Index], PullTargets);
                Warriors.Add(warriorViewModel);
            }

            fsmWave.Fsm.StateCurrent
                .Where(state => state.GetType() == typeof(FsmStateWaveGo))
                .Subscribe(state =>
                {
                    var count = _warriorEntities.Count;
                    //Debug.Log(state.GetType() + " " + count);
                    //Воскрешаем убитых Warrior
                    for (var i = count; i < 3; i++)
                    {
                        CreateWarriorEntity(_freeIndex[0]);
                        _freeIndex.RemoveAt(0);
                    }
                    //TODO Если мобы не под атакой, лечить - в самом WarriorViewModel
                });

            //Инициализация ViewModels


            _warriorEntities.ObserveAdd().Subscribe(e =>
            {
                var warriorEntity = e.Value;
                var warriorViewModel = new WarriorViewModel(
                    warriorEntity, _gameplayState, _towerEntity, 
                    AvailablePath[warriorEntity.Index], PullTargets);
                Warriors.Add(warriorViewModel);
            });
            _warriorEntities.ObserveRemove().Subscribe(e =>
            {
                var warriorEntity = e.Value;
                _freeIndex.Add(warriorEntity.Index);
                foreach (var warriorViewModel in Warriors.ToList())
                {
                    if (warriorViewModel.UniqueId != warriorEntity.UniqueId) continue;
                    Warriors.Remove(warriorViewModel);
                    break;
                }
            });
            
                        
            PullTargets.ObserveAdd().Subscribe(e =>
            {
                //Моб попал в пулл
                var target = e.Value;
                //При его смерти - удаляем из пула
                var disposable = target.IsDead.Where(x => x).Subscribe(_ =>
                {
                    Debug.Log($"Моб умер {target.UniqueId}");
                    PullTargets.Remove(target);
                });
                _mobDisposables.TryAdd(target.UniqueId, disposable); //Кеш подписок на смерть моба
                Debug.Log($"Добавили моба в пулл Башни {target.UniqueId}");
            });
            //При удалении из пула (убит или вышел с дистанции) - удалить из цели
            PullTargets.ObserveRemove().Subscribe(e =>
            {
                var target = e.Value;
                Debug.Log($"Моб удален из Башни {target.UniqueId}");
                if (_mobDisposables.TryGetValue(target.UniqueId, out var disposable))
                {
                    Debug.Log($"disposable {target.UniqueId}");
                    disposable.Dispose();
                }
                _mobDisposables.Remove(target.UniqueId);
            });
        }

        private void UpdateParameterWarrior()
        {
            if (_towerEntity.Parameters.TryGetValue(TowerParameterType.Speed, out var towerSpeed))
                Speed = towerSpeed.Value;
            if (_towerEntity.Parameters.TryGetValue(TowerParameterType.Damage, out var towerDamage))
                Damage = towerDamage.Value;
            if (_towerEntity.Parameters.TryGetValue(TowerParameterType.Health, out var towerHealth))
                Health = towerHealth.Value;
            if (_towerEntity.Parameters.TryGetValue(TowerParameterType.Range, out var towerRange))
                Range = towerRange.Value;
        }

        /**
         * Пересчитываем для каждого индекса Warrior Доступный путь
         */
        private void UpdateWayPath()
        {
            IsWay = _placementService.IsWay(Placement.CurrentValue);

            for (var i = -1; i <= 1; i++)
            {
                AvailablePath[i] = GenerateRoadPoints(i);
            }
            
            //Debug.Log(JsonConvert.SerializeObject(AvailablePath, Formatting.Indented));
        }

        public void UpdateAndRestartWarriors()
        {
            //TODO Обновить солдат
        }

        private void CreateWarriorEntity(int index)
        {
            var warriorEntityData = new WarriorEntityData()
            {
                ParentId = _towerEntity.UniqueId,
                ConfigId = _towerEntity.ConfigId,
                Damage = Damage,
                Health = Health,
                MaxHealth = Health,
                Speed = Speed,
                Range = Range,
                Defence = _towerEntity.Defence,
                IsFly = _towerEntity.TypeEnemy == TowerTypeEnemy.Air,
                PlacementPosition = _towerEntity.Placement.CurrentValue, //Позиция, куда идт warrior первоначально
                StartPosition = _towerEntity.Position.CurrentValue, //Позиция башни, откуда идут warrior
                UniqueId = _gameplayState.CreateEntityID(),
            };
            var warriorEntity = new WarriorEntity(warriorEntityData)
            {
                Index = index
            };
            //При смерти Сущности, сразу удаляем из списка сущностей
            warriorEntity.IsDead.Where(x => x).Subscribe(_ =>
            {
                _warriorEntities.Remove(warriorEntity);
                Debug.Log("warriorEntity.IsDead " + warriorEntity.Index);
            });
            _warriorEntities.Add(warriorEntity);
        }

        public List<RoadPoint> GenerateRoadPoints(int index)
        {
            var delta = DeltaWarrior * index;
            var way = IsWay ? _gameplayState.Origin.Way.ToList() : _gameplayState.Origin.WaySecond.ToList();

            var gate = IsWay ? _gameplayState.GateWave.CurrentValue : _gameplayState.GateWaveSecond.CurrentValue;

            //Последней точкой добавляем Ворота (для direction)
            var rd = new RoadEntityData
            {
                Position = new Vector2Int(
                    (int)Math.Round(gate.x, MidpointRounding.AwayFromZero),
                    (int)Math.Round(gate.y, MidpointRounding.AwayFromZero)
                )
            };
            way.Add(rd);

            var isAvailable = false;
            List<RoadPoint> roads = new();

            //Формируем список точек движения моба
            for (var i = 0; i < way.Count - 1; i++)
            {
                if (way[i].Position == Placement.CurrentValue) isAvailable = true;
                if (!isAvailable) continue;
                
                Vector2 position = way[i].Position; //Определяем новые координаты моба

                //Если координаты дороги выходят за пределы AreaPlacement
                if (position.x > Position.CurrentValue.x + 2 ||
                    position.x < Position.CurrentValue.x - 2 ||
                    position.y > Position.CurrentValue.y + 2 ||
                    position.y < Position.CurrentValue.y - 2)
                    break;
                
                if (way[i].IsTurn)
                {
                    position.x += delta;
                    position.y += delta;
                }
                else
                {
                    if (way[i].Rotate % 2 == 0)
                    {
                        position.y += delta;
                    }
                    else
                    {
                        position.x += delta;
                    }
                }

                var direction = way[i + 1].Position - way[i].Position;
                roads.Add(new RoadPoint(position, direction));
        //        if (index == 0)
       //         {
//                    Debug.Log(position + " => " + direction);
        //        }
            }
            
            return roads;
        }
    }
}