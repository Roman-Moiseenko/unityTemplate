using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Common;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.WarriorStates;
using Game.GamePlay.View.Mobs;
using Game.State.Maps.Mobs;
using Game.State.Maps.Roads;
using Game.State.Maps.Shots;
using Game.State.Maps.Towers;
using Game.State.Maps.Warriors;
using Game.State.Root;
using NUnit.Framework;
using ObservableCollections;
using R3;
using UnityEngine;


namespace Game.GamePlay.View.Warriors
{
    public class WarriorViewModel : IDisposable
    {
        public FsmWarrior FsmWarrior = new();

        private readonly WarriorEntity _warriorEntity;
        public int UniqueId => _warriorEntity.UniqueId;
        public int ParentId => _warriorEntity.ParentId;
        public string ConfigId => _warriorEntity.ConfigId;
        public float Speed => _warriorEntity.Speed;


        public ReactiveProperty<Vector2Int> StartPosition;
        public ReactiveProperty<Vector2Int> PlacementPosition;
        public Vector3 Placement = Vector3.zero;
        public ReactiveProperty<MobViewModel> MobTarget = new();

        public ObservableList<MobViewModel>
            PullAttacks = new(); //Пул мобов которые можно атаковать без поиска из _pullTargets

        public readonly Dictionary<int, IDisposable> MobPullDisposables = new();

        public ReactiveProperty<Vector3> Position = new(Vector3.zero);

        private readonly GameplayStateProxy _gameplayState;
        public float MaxHealth;
        public ReadOnlyReactiveProperty<bool> IsDead => _warriorEntity.IsDead;
        public ReactiveProperty<float> CurrentHealth;
        public int Index => _warriorEntity.Index;
        public bool IsFly => _warriorEntity.IsFly;

        public List<RoadPoint> AvailablePath;
        private readonly ObservableList<MobViewModel> _pullTargets;
        private readonly IDisposable _disposablePullAdd;
        private readonly IDisposable _disposablePullRemove;

        public WarriorViewModel(WarriorEntity warriorEntity, GameplayStateProxy gameplayState, TowerEntity towerEntity,
            List<RoadPoint> availablePath, ObservableList<MobViewModel> pullTargets)
        {
            _warriorEntity = warriorEntity;
            _gameplayState = gameplayState;
            AvailablePath = availablePath;
            MaxHealth = _warriorEntity.Health.CurrentValue;
            CurrentHealth = _warriorEntity.Health;
            _pullTargets = pullTargets;

            StartPosition = towerEntity.Position;
            Position.Value = new Vector3(StartPosition.Value.x, 0, StartPosition.Value.y);
            PlacementPosition = towerEntity.Placement;
            towerEntity.Placement.Subscribe(v =>
            {
                Placement.x = v.x + Index * 0.15f;
                Placement.z = v.y + Index * 0.15f;
            });


            //Подписка на тех мобов которые в цели башни
            _disposablePullAdd = _pullTargets.ObserveAdd().Subscribe(e =>
            {
                var target = e.Value;
                var disposable = target.PositionTarget.Subscribe(v =>
                {
                    //Для воинов которые ожидают, проверяем радиус видимости.
                    if (Vector3.Distance(v, Position.CurrentValue) < AppConstants.WARRIOR_VISIBLE && FsmWarrior.IsAwait())
                        FsmWarrior.Fsm.SetState<FsmWarriorGoToMob>(target); //Движемся к цели
                });
                MobPullDisposables.Add(target.UniqueId, disposable);
            });


            _disposablePullRemove = _pullTargets.ObserveRemove().Subscribe(e =>
            {
                var target = e.Value;
//                Debug.Log($"Моб {target.UniqueId} Вышел из зоны видимости {UniqueId}");
                if (MobPullDisposables.TryGetValue(target.UniqueId, out var disposable))
                {
                    disposable?.Dispose();
                    MobPullDisposables.Remove(target.UniqueId);
                }

                Debug.Log($"Моб {target.UniqueId} удален из _pullTargets"); //Убит
                PullAttacks.Remove(target); //Удаляем из списка атак, чтоб не взять следующей целью

                //При движении к мобу, если цель была убита, ищем новую или возвращаемся
                if (FsmWarrior.IsGoToMob() && FsmWarrior.GetTarget() != null &&
                    FsmWarrior.GetTarget().UniqueId == target.UniqueId)
                    SetTargetMove();

                //Если при атаки данная цель убита, то сбрасываем Цель
                if (MobTarget.CurrentValue != null && MobTarget.CurrentValue.UniqueId == target.UniqueId)
                    MobTarget.OnNext(null);
            });

            PullAttacks.ObserveAdd().Subscribe(e =>
            {
                var target = e.Value;
                
                if (MobTarget.CurrentValue == null) //Первая цель, когда при движении столкнулись с Collider
                {
                    MobTarget.OnNext(target);
                    FsmWarrior.Fsm.SetState<FsmWarriorAttack>(target);
                }
            });

            //Принудительная смена состояния, при достижении определенных точек
            FsmWarrior.Fsm.StateCurrent.Subscribe(state =>
            {
                //Прибыли на базу восстанавливаться
                if (state.GetType() == typeof(FsmWarriorRepair))
                {
                    _warriorEntity.Health.OnNext(MaxHealth);
                    FsmWarrior.Fsm.SetState<FsmWarriorGoToPlacement>();
                }
            });
            
            //Проверяем только, когда цель удалена, чтоб назначить новую или вернуться
            MobTarget.Skip(1).Where(x => x == null).Subscribe(mobTarget =>
            {
                if (FsmWarrior.IsDead()) return; //На всякий случай
                
                Debug.Log("Цель Убита " + FsmWarrior.Fsm.StateCurrent.CurrentValue.GetType());
                //Сначала берем цели из ближащих и их атакуем
                if (PullAttacks.Count > 0)
                {
                    MobTarget.OnNext(PullAttacks[0]); //устанавливаем новую цель
                    FsmWarrior.Fsm.SetState<FsmWarriorAttack>(mobTarget);
                    return;
                }

                //Целей нет, ищем из доступных башни или возвращаемся
                SetTargetMove();
            });

            _warriorEntity.IsDead.Where(x => x).Subscribe(_ =>
            {
                FsmWarrior.Fsm.SetState<FsmWarriorDead>();
                MobTarget.OnNext(null);
            });
        }

        /**
         * Движение закончилось (из Binder), меняем состояние от текущего
         */
        public void IsMovingFinish()
        {
            if (FsmWarrior.IsPlacement()) FsmWarrior.Fsm.SetState<FsmWarriorAwait>();
            if (FsmWarrior.IsGoToRepair()) FsmWarrior.Fsm.SetState<FsmWarriorRepair>();
            if (FsmWarrior.IsGoToMob()) Debug.Log("Исключительная ситуация");
        }

        //При потери цели движения или отсутствии ищем новую или возвращаемся
        private void SetTargetMove()
        {
            //Обходим цели башни и движемся к первой из них
            if (_pullTargets.Count > 0)
            {
                foreach (var target in _pullTargets.ToList())
                {
                    if (Vector3.Distance(target.PositionTarget.CurrentValue, Placement) >= AppConstants.WARRIOR_VISIBLE) 
                        continue;
                    FsmWarrior.Fsm.SetState<FsmWarriorGoToMob>(target);
                    return;
                }
            }

            //Целей больше нет, возвращаемся ...
            if (CurrentHealth.CurrentValue < MaxHealth) // лечиться
            {
                FsmWarrior.Fsm.SetState<FsmWarriorGoToRepair>();
            }
            else //На базу Placement
            {
                FsmWarrior.Fsm.SetState<FsmWarriorGoToPlacement>();
            }
        }

        private void RemoveTarget(MobViewModel mobViewModel)
        {
            if (MobTarget.CurrentValue == mobViewModel) MobTarget.OnNext(null);
        }

        public void ClearTarget()
        {
            MobTarget.OnNext(null);
        }

        public void SetDamageAfterShot()
        {
            //Доп.проверка на случай убийства моба
            if (MobTarget.CurrentValue == null) return;
            var shot = new ShotData
            {
                Damage = _warriorEntity.Damage,
                DamageType = DamageType.Normal,
                Position = MobTarget.CurrentValue.PositionTarget.CurrentValue,
                Single = true,
                MobEntityId = MobTarget.CurrentValue.UniqueId,
            };
            
//            Debug.Log("Урон от Warrior " + UniqueId + " Мобу " + MobTarget.CurrentValue.UniqueId);
            _gameplayState.Shots.Add(shot);
        }

        public void DamageWarrior(float damage, MobDefence defence)
        {
            if (_warriorEntity.Defence.Previous() == defence) damage *= 0.8f;
            if (_warriorEntity.Defence.Next() == defence) damage *= 1.2f;

            _warriorEntity.DamageReceived(damage);
        }

        public void Dispose()
        {
            //StartPosition?.Dispose();
            //PlacementPosition?.Dispose();
            //MobTarget?.Dispose();
            //Position?.Dispose();
            MobTarget?.Dispose();
            _disposablePullAdd.Dispose();
            _disposablePullRemove.Dispose();
            //CurrentHealth?.Dispose();
            foreach (var (key, disposable) in MobPullDisposables.ToList())
            {
                disposable?.Dispose();
                MobPullDisposables.Remove(key);
            }

            FsmWarrior.Fsm.Dispose();
        }
    }
}