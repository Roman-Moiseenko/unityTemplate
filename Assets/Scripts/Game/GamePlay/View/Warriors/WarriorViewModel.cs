using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public ObservableList<MobViewModel> PullAttacks = new(); //Пул мобов до которых надо дойти
        public ObservableList<MobViewModel> PullTargets = new(); //Пул мобов которых надо атаковать
        public readonly Dictionary<int, IDisposable> MobPullDisposables = new();
        public readonly Dictionary<int, IDisposable> MobDisposables = new();
        public ReactiveProperty<Vector3> Position = new(Vector3.zero);

        private readonly GameplayStateProxy _gameplayState;
        public float MaxHealth;
        public ReadOnlyReactiveProperty<bool> IsDead => _warriorEntity.IsDead;
        public ReactiveProperty<float> CurrentHealth;
        public int Index => _warriorEntity.Index;
        public bool IsFly => _warriorEntity.IsFly;

        public List<RoadPoint> AvailablePath;
        private readonly ObservableList<MobViewModel> _pullTargets;
        private IDisposable _disposablePullAdd;
        private IDisposable _disposablePullRemove;

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
                if (FsmWarrior.IsAwait())
                {
                    //   FsmWarrior.Fsm.SetState<FsmWarriorGoToMob>(target);//Движемся к цели
                }

                var disposable = target.PositionTarget.Subscribe(v =>
                {
                   // Debug.Log(Vector3.Distance(v, Position.CurrentValue) + " " + v + " => " + Position.CurrentValue);
                    //  SetTargetMove();
                    //Радиус видимости
                    if (Vector3.Distance(v, Position.CurrentValue) < 2.5f && FsmWarrior.IsAwait())
                    {
                        Debug.Log($"Моб {target.UniqueId} В зоне видимости {UniqueId}");
                        //К мобу
                        FsmWarrior.Fsm.SetState<FsmWarriorGoToMob>(target); //Движемся к цели
                        // MobTarget.OnNext(target); 
                    }
                });

                MobPullDisposables.Add(target.UniqueId, disposable);
            });


            _disposablePullRemove = _pullTargets.ObserveRemove().Subscribe(e =>
            {
                var target = e.Value;
                Debug.Log($"Моб {target.UniqueId} Вышел из зоны видимости {UniqueId}");
                if (MobPullDisposables.TryGetValue(target.UniqueId, out var disposable))
                {
                    disposable?.Dispose();
                    MobPullDisposables.Remove(target.UniqueId);
                }

                PullAttacks.Remove(target);
                if (FsmWarrior.GetTarget().UniqueId == target.UniqueId)
                {
                    SetTargetMove();
                }

                Debug.Log("Удалили из PullAttacks");
            });

            PullAttacks.ObserveAdd().Subscribe(e =>
            {
                var target = e.Value;
                Debug.Log($"Моб {target.UniqueId} В зоне поражения {UniqueId}");

                //Удаляем из целей для Атаки (те, кто уже вошел в область атаки)
                var disposable = target.IsDead
                    .Where(x => x)
                    .Subscribe(_ =>
                    {
                        Debug.Log("моб мертв " + target.UniqueId);
                        PullAttacks.Remove(target);
                        //Если моб был целью тек.атаки, обнуляем цель
                        if (MobTarget.CurrentValue.UniqueId == target.UniqueId) MobTarget.OnNext(null);
                    });
                MobDisposables.TryAdd(target.UniqueId, disposable); //Кеш подписок на смерть моба
                if (MobTarget.CurrentValue == null) MobTarget.OnNext(target); //Первая цель
            });

            PullAttacks.ObserveRemove().Subscribe(e =>
            {
                var target = e.Value;
                Debug.Log($"Моб {target.UniqueId} Вышел из зоны поражения {UniqueId}");
                if (MobDisposables.TryGetValue(target.UniqueId, out var disposable))
                {
                    disposable?.Dispose();
                    MobDisposables.Remove(target.UniqueId);
                }
            });


            FsmWarrior.Fsm.StateCurrent.Subscribe(state =>
            {
                //Когда прибыл на точку спавна, проверяем здоровье, если не хватает - в башню
                if (state.GetType() == typeof(FsmWarriorAwait) && CurrentHealth.CurrentValue < MaxHealth)
                    FsmWarrior.Fsm.SetState<FsmWarriorGoToRepair>();

                //
                if (state.GetType() == typeof(FsmWarriorRepair))
                {
                    Debug.Log($"{UniqueId} Восстановились");
                    _warriorEntity.Health.OnNext(MaxHealth);
                    FsmWarrior.Fsm.SetState<FsmWarriorToPlacement>();
                }
            });


            MobTarget.Skip(1).Subscribe(mobTarget =>
            {
                if (mobTarget == null) //Цель убита
                {
                    Debug.Log("Цель Убита " + FsmWarrior.Fsm.StateCurrent.CurrentValue.GetType());
                    //Сначала берем цели из ближащих и атакуем
                    if (PullAttacks.Count > 0)
                    {
                        Debug.Log($"Новая цель для {UniqueId} из PullAttacks");
                        MobTarget.OnNext(PullAttacks[0]); //, устанавливаем новую цель
                        return;
                    }

                    Debug.Log("SetTargetMove");
                    SetTargetMove();
                }
                else //Цель назначена, движемся к цели
                {
                    Debug.Log($"Включаем атаку на моба {mobTarget.UniqueId}");
                    FsmWarrior.Fsm.SetState<FsmWarriorAttack>(mobTarget);
                }
            });

            _warriorEntity.IsDead.Where(x => x).Subscribe(_ =>
            {
                FsmWarrior.Fsm.SetState<FsmWarriorDead>();


                if (FsmWarrior.Fsm.Params == null)
                {
                    Debug.Log("FsmWarrior.Fsm.Params == null");
                }
                else
                {
                    Debug.Log(FsmWarrior.GetTarget().UniqueId);
                }
            });
        }

        //При потери цели движения или отсутствии ищем новую или возвращаемся
        private void SetTargetMove()
        {
            //Обходим цели башни и движемся к первой из них
            if (_pullTargets.Count > 0)
            {
                foreach (var pullTarget in _pullTargets.ToList())
                {
                    if (Vector3.Distance(pullTarget.PositionTarget.CurrentValue, Placement) < 2.5f)
                    {
                        Debug.Log($"Новая цель для {UniqueId} из pullTargets {pullTarget.UniqueId}");
                        FsmWarrior.Fsm.SetState<FsmWarriorAwait>();
                        FsmWarrior.Fsm.SetState<FsmWarriorGoToMob>(pullTarget);
                        return;
                    }
                }
            }

            //Целей больше нет, возвращаемся
            Debug.Log($"Возвращаемся {UniqueId}");
            FsmWarrior.Fsm.SetState<FsmWarriorToPlacement>();
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

        //public void 

        public void Dispose()
        {
            //StartPosition?.Dispose();
            //PlacementPosition?.Dispose();
            //MobTarget?.Dispose();
            //Position?.Dispose();
            _disposablePullAdd.Dispose();
            _disposablePullRemove.Dispose();
            //CurrentHealth?.Dispose();
            Debug.Log("_warriorEntity IsDead " + MobPullDisposables.Count);
            foreach (var (key, disposable) in MobPullDisposables.ToList())
            {
                disposable?.Dispose();
                MobPullDisposables.Remove(key);
            }

            Debug.Log("_warriorEntity MobPullDisposables " + MobDisposables.Count);
            foreach (var (key, disposable) in MobDisposables.ToList())
            {
                disposable?.Dispose();
                MobDisposables.Remove(key);
            }

            Debug.Log("_warriorEntity MobDisposables");
            FsmWarrior.ClearParams();
        }
    }
}