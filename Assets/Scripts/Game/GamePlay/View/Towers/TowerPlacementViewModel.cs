using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.Common;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.WaveStates;
using Game.GamePlay.Services;
using Game.GamePlay.View.Mobs;
using Game.GamePlay.View.Warriors;
using Game.State.Maps.Roads;
using Game.State.Maps.Towers;
using Game.State.Maps.Warriors;
using Game.State.Root;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerPlacementViewModel : TowerViewModel
    {
        private const int CountWarriors = 3;
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
        
        public Dictionary<int, RoadPoint3> AvailablePath = new();

        private List<IDisposable> _disposables = new();

        //Кеш подписок на смерть моба
        private readonly Dictionary<int, IDisposable> _mobDisposables = new();
        public ReactiveProperty<Vector2Int> Placement => TowerEntity.Placement;
        public bool IsFly { get; set; }

        public TowerPlacementViewModel(TowerEntity towerEntity, DIContainer container, FsmWave fsmWave, PlacementService placementService) : base(towerEntity, container)
        {
            IsFly = TowerEntity.TypeEnemy == TowerTypeEnemy.Air;
            _placementService = placementService;
            UpdateParameterWarrior();
            for (var i = 1; i <= CountWarriors; i++)
                CreateWarriorEntity(i - 2);

            //При изменении точки спавна и обновлении дорог, меняем доступный путь Warriors
            Placement.Subscribe(_ => UpdateWayPath());
            var d1 = GameplayState.Way.ObserveAdd().Subscribe(_ => UpdateWayPath());
            _disposables.Add(d1);
            var d2 = GameplayState.WaySecond.ObserveAdd().Subscribe(_ => UpdateWayPath());
            _disposables.Add(d2);

            foreach (var warriorEntity in _warriorEntities)
            {
                var warriorViewModel = new WarriorViewModel(
                    warriorEntity, GameplayState, TowerEntity, AvailablePath[warriorEntity.Index], PullTargets);
                Warriors.Add(warriorViewModel);
            }

            var d3 = fsmWave.Fsm.StateCurrent
                .Where(state => state.GetType() == typeof(FsmStateWaveBegin))
                .Subscribe(state =>
                {
                    var count = _warriorEntities.Count;

                    //Воскрешаем убитых Warrior
                    for (var i = count; i < CountWarriors; i++)
                    {
                        CreateWarriorEntity(_freeIndex[0]);
                        _freeIndex.RemoveAt(0);
                    }
                });
            _disposables.Add(d3);
            //Инициализация ViewModels
            
            _warriorEntities.ObserveAdd().Subscribe(e =>
            {
                var warriorEntity = e.Value;
                var warriorViewModel = new WarriorViewModel(
                    warriorEntity, GameplayState, TowerEntity,
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
                //  Debug.Log($"Добавили моба в пулл Башни {target.UniqueId}");
                //При его смерти - удаляем из пула
                var disposable = target.IsDead.Where(x => x).Subscribe(_ =>
                {
                    // Debug.Log($"Моб умер {target.UniqueId}");
                    PullTargets.Remove(target);
                });
                _mobDisposables.TryAdd(target.UniqueId, disposable); //Кеш подписок на смерть моба
            });
            //При удалении из пула (убит или вышел с дистанции) - удалить из цели
            PullTargets.ObserveRemove().Subscribe(e =>
            {
                var target = e.Value;
                // Debug.Log($"Моб удален из Башни {target.UniqueId}");
                if (_mobDisposables.TryGetValue(target.UniqueId, out var disposable))
                {
                    disposable.Dispose();
                    _mobDisposables.Remove(target.UniqueId);
                }
            });
            var d4 = Level.Subscribe(level =>
            {
                UpdateParameterWarrior();
                UpdateAndRestartWarriors();
            });
            _disposables.Add(d4);
        }

        private void UpdateParameterWarrior()
        {
            if (TowerEntity.Parameters.TryGetValue(TowerParameterType.Speed, out var towerSpeed))
                Speed = towerSpeed.Value;
            if (TowerEntity.Parameters.TryGetValue(TowerParameterType.Damage, out var towerDamage))
                Damage = towerDamage.Value;
            if (TowerEntity.Parameters.TryGetValue(TowerParameterType.Health, out var towerHealth))
                Health = towerHealth.Value;
            if (TowerEntity.Parameters.TryGetValue(TowerParameterType.Range, out var towerRange))
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

        private void UpdateAndRestartWarriors()
        {
            //Обновить солдат
            foreach (var warriorEntity in _warriorEntities)
            {
                warriorEntity.Damage.OnNext(Damage);
                warriorEntity.Health.OnNext(Health);
                warriorEntity.Speed.OnNext(Speed);
                warriorEntity.MaxHealth.OnNext(Health);
            }
        }

        private void CreateWarriorEntity(int index)
        {
            var warriorEntityData = new WarriorEntityData()
            {
                ParentId = TowerEntity.UniqueId,
                ConfigId = TowerEntity.ConfigId,
                Damage = Damage,
                Health = Health,
                MaxHealth = Health,
                Speed = Speed,
                Range = Range,
                Defence = TowerEntity.Defence,
                IsFly = TowerEntity.TypeEnemy == TowerTypeEnemy.Air,
                PlacementPosition = TowerEntity.Placement.CurrentValue, //Позиция, куда идт warrior первоначально
                StartPosition = TowerEntity.Position.CurrentValue, //Позиция башни, откуда идут warrior
                UniqueId = GameplayState.CreateEntityID(),
            };
            var warriorEntity = new WarriorEntity(warriorEntityData)
            {
                Index = index
            };
            //При смерти Сущности, сразу удаляем из списка сущностей
            warriorEntity.IsDead.Where(x => x).Subscribe(_ => _warriorEntities.Remove(warriorEntity));
            _warriorEntities.Add(warriorEntity);
        }

        public RoadPoint3 GenerateRoadPoints(int index)
        {
            var delta = DeltaWarrior * index;
            var way = IsWay ? GameplayState.Origin.Way.ToList() : GameplayState.Origin.WaySecond.ToList();

            var gate = IsWay ? GameplayState.GateWave.CurrentValue : GameplayState.GateWaveSecond.CurrentValue;

            //Последней точкой добавляем Ворота (для direction)
            var rd = new RoadEntityData
            {
                Position = new Vector2Int(
                    (int)Math.Round(gate.x, MidpointRounding.AwayFromZero),
                    (int)Math.Round(gate.y, MidpointRounding.AwayFromZero)
                )
            };
            way.Add(rd);


            //Формируем список точек движения моба
            for (var i = 0; i < way.Count - 1; i++)
            {
                if (way[i].Position == Placement.CurrentValue)
                {
                    var result = new RoadPoint3();

                    var direction = MyFunc.Vector2To3(way[i + 1].Position) - MyFunc.Vector2To3(way[i].Position); 
                    result.Direction = direction.normalized;
/*
                    if (direction.y == 0)
                    {
                        
                    }
                    else if(direction.x == 0)
                    {
                        
                    }
                    
                    Debug.Log(way[i + 1].Position + " " + way[i].Position);
                    Debug.Log(result.Direction.normalized);*/
                    Vector2 position = way[i + 1].Position;
                    
                    if (position.x > Position.CurrentValue.x + 2 ||
                        position.x < Position.CurrentValue.x - 2 ||
                        position.y > Position.CurrentValue.y + 2 ||
                        position.y < Position.CurrentValue.y - 2)
                    {
                        result.Point = null;
                    }
                    else
                    {
                        position.x += delta;
                        position.y += delta;
                        result.Point = MyFunc.Vector2To3(position);
                    }

                    return result;
                }
            }

            throw new Exception("Точкеа на дороге не нашлась");
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }
        }
    }
}