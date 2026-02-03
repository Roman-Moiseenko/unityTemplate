using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.Commands.WarriorCommands;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.TowerStates;
using Game.GamePlay.Services;
using Game.GamePlay.View.Frames;
using Game.GamePlay.View.Mobs;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Maps.Mobs;
using Game.State.Maps.Shots;
using Game.State.Maps.Towers;
using Game.State.Root;
using MVVM.CMD;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerViewModel : IMovingEntityViewModel
    {
        private readonly GameplayStateProxy _gameplayState;
        private readonly TowersService _towersService;
        private readonly TowerEntity _towerEntity;
        public bool IsPlacement => _towerEntity.IsPlacement; 
        public Dictionary<TowerParameterType, TowerParameterData> Parameters => _towerEntity.Parameters;
        public readonly int UniqueId;
        public ReactiveProperty<int> Level { get; set; }
        public readonly string ConfigId;
        public ReactiveProperty<Vector2Int> Position { get; set; }
        public ReactiveProperty<Vector3> PositionMap = new();
        public bool IsOnRoad => _towerEntity.IsOnRoad;
        public ReactiveProperty<bool> IsShot;
        public ReactiveProperty<Vector3> Direction = new();
        public float Speed = 0f;
        public ReactiveProperty<int> NumberModel = new(0);
        public float SpeedShot => _towerEntity.SpeedShot;
        public bool IsMultiShot => _towerEntity.IsMultiShot;

        public bool IsSingleTarget => _towerEntity.IsSingleTarget;
        private IMovingEntityViewModel _movingEntityViewModelImplementation;
        
        public ReactiveProperty<float> MaxDistance = new(0f);
        public float MinDistance = 0f;

        public ObservableDictionary<int, MobViewModel> MobTargets = new();

        public ReactiveProperty<bool> FinishEffectLevelUp = new(false);
        public ReactiveProperty<bool> ShowArea = new(false);

        public ReactiveProperty<Vector2Int> Placement => _towerEntity.Placement;
        
        public ObservableList<MobViewModel> PullTargets = new();

        //Флаг для передачи в Панели подтверждения из различных состояния
        public ReactiveProperty<bool> IsConfirmationState = new(true);
        
        //Кеш подписок на смерть моба
        private readonly Dictionary<int, IDisposable> _mobDisposables = new();
        
        public TowerViewModel(
            TowerEntity towerEntity,
            GameplayStateProxy gameplayState,
            TowersService towersService,
            FsmTower fsmTower
        )
        {
            _gameplayState = gameplayState;
            _towersService = towersService;
            _towerEntity = towerEntity;   
            
            IsShot = towerEntity.IsShot;
            UniqueId = towerEntity.UniqueId;
            ConfigId = towerEntity.ConfigId;
            Level = towerEntity.Level;
            Position = towerEntity.Position;

            Position.Subscribe(v => PositionMap.Value = new Vector3(v.x, 0, v.y));

            if (towerEntity.Parameters.TryGetValue(TowerParameterType.Speed, out var towerSpeed))
                Speed = towerSpeed.Value;

            if (towerEntity.Parameters.TryGetValue(TowerParameterType.MinDistance, out var towerMinDistance))
                MinDistance = towerMinDistance.Value;
            
            Level.Subscribe(level =>
            {
                
                if (towerEntity.Parameters.TryGetValue(TowerParameterType.Distance, out var towerDistance))
                    MaxDistance.Value = towerDistance.Value;
                if (towerEntity.Parameters.TryGetValue(TowerParameterType.MaxDistance, out var towerMaxDistance))
                    MaxDistance.Value = towerMaxDistance.Value;
                
                //Смена модели
                NumberModel.Value = level switch
                {
                    1 or 2 => 1,
                    3 or 4 => 2,
                    5 or 6 => 3,
                    _ => throw new Exception("Неизвестный уровень")
                };
            });
            fsmTower?.Fsm.StateCurrent.Subscribe(state =>
            {
                if (state.GetType() == typeof(FsmTowerNone)) ShowArea.Value = false;
                if (state.GetType() == typeof(FsmTowerSelected) && fsmTower.GetTowerViewModel().UniqueId == UniqueId)
                    ShowArea.Value = true;
            });

            //** Логика ведения целей **//
            PullTargets.ObserveAdd().Subscribe(e =>
            {
                //Моб попал в пулл
                var target = e.Value;
                //При его смерти - удаляем из пула
                var disposable = target.IsDead.Where(x => x).Subscribe(_ => PullTargets.Remove(target));
                _mobDisposables.Add(target.UniqueId, disposable); //Кеш подписок на смерть моба
                SetTarget(target); //Добавляем его цель (если мультишот, то добавляется, для одиночного идет проверка)
            });

            //При удалении из пула (убит или вышел с дистанции) - удалить из цели
            PullTargets.ObserveRemove().Subscribe(e =>
            {
                var target = e.Value;
                _mobDisposables.Remove(target.UniqueId);
                MobTargets.Remove(target.UniqueId);
            });

            //При удалении из цели, попытка добавить из пулла
            MobTargets.ObserveRemove().Subscribe(e =>
            {
                //При мультишоте цель автоматически добавляется при попадании в Пулл
                if (!IsMultiShot && PullTargets.Count > 0) SetTarget(PullTargets[0]); //Первый из списка
            });

        }
        public bool IsPosition(Vector2 position)
        {
            const float delta = 0.5f; //Половина ширины клетки
            var _x = Position.CurrentValue.x;
            var _y = Position.CurrentValue.y;
            if ((position.x >= _x - delta && position.x <= _x + delta) &&
                (position.y >= _y - delta && position.y <= _y + delta))
                return true;
            return false;
        }

        public void SetPosition(Vector2Int position)
        {
            //
        }

        public Vector2Int GetPosition()
        {
            return Position.CurrentValue;
        }

        public Vector3 GetAreaRadius()
        {
            if (_towerEntity.IsPlacement) return new Vector3(5, 5, 1); //Scale для модели
            
            var radius = Vector3.zero; //Zero для башен без области
            if (_towerEntity.Parameters.TryGetValue(TowerParameterType.MinDistance, out var min))
                radius.y = min.Value;
            if (_towerEntity.Parameters.TryGetValue(TowerParameterType.MaxDistance, out var max))
                radius.x = max.Value;
            if (_towerEntity.Parameters.TryGetValue(TowerParameterType.Distance, out var parameter))
                radius.x = parameter.Value;
            
            //TODO Если к башне применен параметр Высота (+дистанции) то вычисляем radius.z = %% от radius.x
            return radius;
        }

        public void SetDirection(Vector2Int direction)
        {
            Direction.Value = new Vector3(direction.x, 0, direction.y);
        }
        
        private void SetTarget(MobViewModel viewModel)
        {
            if (!IsMultiShot && MobTargets.Count != 0) return;
            
            if (MobTargets.TryGetValue(viewModel.UniqueId, out var value)) return;
            MobTargets.TryAdd(viewModel.UniqueId, viewModel);

        }

        private void RemoveTarget(MobViewModel mobBinderViewModel)
        {
            MobTargets.Remove(mobBinderViewModel.UniqueId);
        }

        public void ClearTargets()
        {
            foreach (var (key, mobViewModel) in MobTargets.ToArray())
            {
                RemoveTarget(mobViewModel);
            }
        }
        
        /**
         * Башня наносящая урон
         */
        public void SetDamageAfterShot(MobViewModel mobViewModel)
        {
            var shot = _towerEntity.GetShotParameters(mobViewModel.Defence);
            shot.MobEntityId = mobViewModel.UniqueId;
            _gameplayState.Shots.Add(shot); 
        }

        /**
         * Башня призывающая воинов
         */
        public bool IsDeadAllWarriors()
        {
            return _towersService.IsDeadAllWarriors(_towerEntity);
        }
        public void AddWarriorsTower()
        {
            _towersService.AddWarriorsTower(_towerEntity);
        }

        /**
         * Проверяем на совместимость Башни и моба для нанесения урона
         */
        public bool IsTargetForDamage(bool mobIsFly)
        {
            switch (_towerEntity.TypeEnemy)
            {
                case TowerTypeEnemy.Universal:
                case TowerTypeEnemy.Air when mobIsFly:
                case TowerTypeEnemy.Ground when !mobIsFly:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsInPlacement(Vector2Int position)
        {
            if (IsPlacement == false) return false;

            return Math.Abs(Position.CurrentValue.x - position.x) < 3 && 
                   Math.Abs(Position.CurrentValue.y - position.y) < 3;
        }
    }
}