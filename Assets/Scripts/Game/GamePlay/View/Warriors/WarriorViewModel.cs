using System;
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
        public float Speed => _warriorEntity.Speed.Value;

        public ReactiveProperty<Vector2Int> StartPosition;
        public ReactiveProperty<Vector2Int> PlacementPosition;
        public Vector3 Placement = Vector3.zero;
        public ReactiveProperty<MobViewModel> MobTarget = new();

       // public ObservableList<MobViewModel> PullAttacks = new(); //Пул мобов которые можно атаковать без поиска из _pullTargets
        public readonly Dictionary<int, IDisposable> MobPullDisposables = new();
        public ReactiveProperty<Vector3> Position = new(Vector3.zero);

        private readonly GameplayStateProxy _gameplayState;
        public ReactiveProperty<float> MaxHealth => _warriorEntity.MaxHealth;
        public ReadOnlyReactiveProperty<bool> IsDead => _warriorEntity.IsDead;
        public ReactiveProperty<float> CurrentHealth => _warriorEntity.Health;
        public int Index => _warriorEntity.Index;
        public bool IsFly => _warriorEntity.IsFly;
        public RoadPoint3 AvailablePath;


        private readonly ObservableList<MobViewModel> _pullTargets;
        private readonly IDisposable _disposablePullAdd;
        private readonly IDisposable _disposablePullRemove;
        private MobViewModel _targetMove;

        public WarriorViewModel(WarriorEntity warriorEntity, GameplayStateProxy gameplayState, TowerEntity towerEntity,
            RoadPoint3 availablePath, ObservableList<MobViewModel> pullTargets)
        {
            _warriorEntity = warriorEntity;
            _gameplayState = gameplayState;
            AvailablePath = availablePath;
            _pullTargets = pullTargets;

            StartPosition = towerEntity.Position;
            Position.Value = new Vector3(StartPosition.Value.x, 0, StartPosition.Value.y);
            PlacementPosition = towerEntity.Placement;
            towerEntity.Placement.Subscribe(v =>
            {
                if (AvailablePath.Direction.z == 0)
                {
                    Placement.z = v.y + Index * AppConstants.WARRIOR_DELTA;
                    Placement.x = v.x - (AvailablePath.Direction.x * Math.Abs(Index * AppConstants.WARRIOR_DELTA));
                }
                else if (AvailablePath.Direction.x == 0)
                {
                    Placement.x = v.x + Index * AppConstants.WARRIOR_DELTA;
                    Placement.z = v.y - (AvailablePath.Direction.z * Math.Abs(Index * AppConstants.WARRIOR_DELTA));
                }
            });


            //Подписка на тех мобов которые в цели башни
            _disposablePullAdd = _pullTargets.ObserveAdd().Subscribe(e =>
            {
                var target = e.Value;
                //Отслеживаем расстояние для всех возможных целей башни, каждым Warrior
                var disposable = target.PositionTarget.Subscribe(v =>
                {
                    if (MobTarget.Value != null) return; //Не проверяем расстояние, если есть цель
                    SetMoveOrAttackTarget(target);
                    
                    //Проверяем, если цель близко, то назначаем цель 
                   /* if (Vector3.Distance(v, Position.CurrentValue) < 0.25f)  //&& !FsmWarrior.IsAttack()
                    {
                        MobTarget.Value = target;
                        FsmWarrior.Fsm.SetState<FsmWarriorAttack>();
                        return;
                    }
                    
                    //Для воинов которые ожидают, проверяем радиус видимости.
                    if (Vector3.Distance(v, Position.CurrentValue) < AppConstants.WARRIOR_VISIBLE &&
                        FsmWarrior.IsAwait())
                    {
                        _targetMove = target;
                        if (Vector3.Distance(_targetMove.PositionTarget.CurrentValue, Position.CurrentValue) < 1f)
                        {
                            FsmWarrior.Fsm.SetState<FsmWarriorGoToMob>(_targetMove.PositionTarget
                                .CurrentValue); //Движемся к цели
                        }
                        else
                        {
                            FsmWarrior.Fsm.SetState<FsmWarriorGoToMob>(AvailablePath.Point); //Движемся к 1 точке пути
                        }
                        return;
                    }
                    */
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

                // Debug.Log($"Моб {target.UniqueId} удален из _pullTargets"); //Убит
                //PullAttacks.Remove(target); //Удаляем из списка атак, чтоб не взять следующей целью

                //При движении к мобу, если цель была убита, ищем новую или возвращаемся
                if ( /*FsmWarrior.IsGoToMob() && */_targetMove != null && _targetMove.UniqueId == target.UniqueId)
                {
                    _targetMove = null;
                }
                SetTargetOrReturnToBase();

                //Если при атаки данная цель убита, то сбрасываем Цель
                if (MobTarget.CurrentValue != null && MobTarget.CurrentValue.UniqueId == target.UniqueId)
                    MobTarget.Value = null;
                
                
                
                //Доп.проверка
                if (_pullTargets.Count == 0)
                {
                  //  PullAttacks.Clear();
                    MobTarget.Value = null;
                    _targetMove = null;
                }
            });

            
            /*
            PullAttacks.ObserveAdd().Subscribe(e =>
            {
                var target = e.Value;

                Debug.Log(UniqueId + " Add " + PullAttacks.Count);
                if (MobTarget.CurrentValue == null &&
                    !target.IsDead.CurrentValue) //Первая цель, когда при движении столкнулись с Collider
                {
                    MobTarget.Value = target;
                    FsmWarrior.Fsm.SetState<FsmWarriorAttack>(target.PositionTarget.CurrentValue);
                }
            });
            PullAttacks.ObserveRemove().Subscribe(e =>
            {
                Debug.Log(UniqueId + " Remove " + e.Value.UniqueId + " Count = "+ PullAttacks.Count);
            });
            PullAttacks.ObserveClear().Subscribe(_ => MobTarget.Value = null);
            */
            //Принудительная смена состояния, при достижении определенных точек
            FsmWarrior.Fsm.StateCurrent.Subscribe(state =>
            {
                //Прибыли на базу восстанавливаться
                if (state.GetType() == typeof(FsmWarriorRepair))
                {
                    _warriorEntity.Health.OnNext(MaxHealth.CurrentValue);
                    FsmWarrior.Fsm.SetState<FsmWarriorGoToPlacement>(Placement);
                }
            });

            //Проверяем только, когда цель удалена, чтоб назначить новую или вернуться
     /*       MobTarget.Skip(1).Where(x => x == null).Subscribe(mobTarget =>
            {
                if (FsmWarrior.IsDead()) return; //На всякий случай

//                Debug.Log("Цель Убита " + FsmWarrior.Fsm.StateCurrent.CurrentValue.GetType());
                //Сначала берем цели из ближащих и их атакуем
                if (PullAttacks.Count > 0)
                {
                    MobTarget.Value = PullAttacks[0]; //устанавливаем новую цель
                    FsmWarrior.Fsm.SetState<FsmWarriorAttack>(MobTarget.CurrentValue.PositionTarget.CurrentValue);
                    return;
                }

          //      Debug.Log("PullAttacks.Count " + PullAttacks.Count);
                //Целей нет, ищем из доступных башни или возвращаемся
              //  SetTargetOrReturnToBase();
            });*/

            _warriorEntity.IsDead.Where(x => x).Subscribe(_ =>
            {
                FsmWarrior.Fsm.SetState<FsmWarriorDead>();
                MobTarget.Value = null;
            });
        }

        private bool SetMoveOrAttackTarget(MobViewModel mobViewModel)
        {
            var mobPosition = mobViewModel.PositionTarget.CurrentValue;
            if (MobTarget.Value != null) return true; //Не проверяем расстояние, если есть цель
                    
            //TODO Взять данные от Warrior
            //Проверяем, если цель близко, то назначаем цель для атаки
            if (Vector3.Distance(mobPosition, Position.CurrentValue) < 0.5f)  //&& !FsmWarrior.IsAttack()
            {
                MobTarget.Value = mobViewModel;
                FsmWarrior.Fsm.SetState<FsmWarriorAttack>(mobViewModel.PositionTarget.CurrentValue);
                return true;
            }

            if (_targetMove != null) return true; //Есть цель для движения
            
            //Для воинов которые ожидают, проверяем радиус видимости.
            if (Vector3.Distance(mobPosition, Position.CurrentValue) < AppConstants.WARRIOR_VISIBLE)
            {
                _targetMove = mobViewModel;
                if (Vector3.Distance(_targetMove.PositionTarget.CurrentValue, Position.CurrentValue) < 1.75f)
                {
                    FsmWarrior.Fsm.SetState<FsmWarriorGoToMob>(_targetMove.PositionTarget.CurrentValue); //Движемся к цели
                }
                else
                {
                    FsmWarrior.Fsm.SetState<FsmWarriorGoToMob>(AvailablePath.Point); //Движемся к 1 точке пути
                }
                return true;
            }

            return false;
        }
        
        
        /**
         * Движение закончилось (из Binder), меняем состояние от текущего
         */
        public void IsMovingFinish(Vector3 position)
        {
            if (FsmWarrior.IsPlacement())
            {
                if (position == Placement)
                {
                    //     Debug.Log("==");
                    FsmWarrior.Fsm.SetState<FsmWarriorAwait>();
                }
                else //дошли до 1й точки
                {
                    //  Debug.Log("position != Placement " + position);
                    //var pos = MyFunc.Vector2To3(Placement);
                    FsmWarrior.Fsm.SetState<FsmWarriorGoToPlacement>(Placement);
                }
            }

            if (FsmWarrior.IsGoToRepair()) FsmWarrior.Fsm.SetState<FsmWarriorRepair>();
            if (FsmWarrior.IsGoToMob())
            {
                SetTargetPositionForGoToMob(position);
                //Меняем точки движения
                // Debug.Log("Исключительная ситуация");
            }
        }

        private void SetTargetPositionForGoToMob(Vector3? position)
        {
            // Debug.Log("SetTargetPositionForGoToMob ");

            if (position == null || Vector3.Distance(_targetMove.PositionTarget.CurrentValue, (Vector3)position) < 1)
            {
                //Идем до моба
                //    Debug.Log("= Distance = " + _targetMove.PositionTarget.CurrentValue);
                FsmWarrior.Fsm.SetState<FsmWarriorGoToMob>(_targetMove.PositionTarget.CurrentValue);
            }
            else
            {
                //Идем до 1й точки
                //        Debug.Log("AvailablePath.Point " + AvailablePath.Point);
                FsmWarrior.Fsm.SetState<FsmWarriorGoToMob>(AvailablePath.Point);
            }
        }

        //При потере цели движения или отсутствии ищем новую или возвращаемся на базу
        private void SetTargetOrReturnToBase()
        {
            //Обходим цели башни и движемся к первой из них
            if (_pullTargets.Count > 0)
            {
                foreach (var target in _pullTargets.ToList())
                {
                    //Цель была назначена, выходим
                    if (SetMoveOrAttackTarget(target)) return;
                    /*
                    if (Vector3.Distance(target.PositionTarget.CurrentValue, Placement) >= AppConstants.WARRIOR_VISIBLE)
                        continue;
                    _targetMove = target;
                    SetTargetPositionForGoToMob(AvailablePath.Point);
                    
                    return;*/
                }
            }

            //Целей больше нет, возвращаемся ...
            if (CurrentHealth.CurrentValue < MaxHealth.CurrentValue) // лечиться, по прямой
            {
                Debug.Log(" _pullTargets.Count " + _pullTargets.Count);
                
                FsmWarrior.Fsm.SetState<FsmWarriorGoToRepair>();
            }
            else //На базу Placement, по дороге, через точку поворота
            {
                //Проверить где Warrior
                if (Vector3.Distance(Position.CurrentValue, Placement) < 1.1f)
                {
                    FsmWarrior.Fsm.SetState<FsmWarriorGoToPlacement>(Placement);
                }
                else
                {
                    FsmWarrior.Fsm.SetState<FsmWarriorGoToPlacement>(AvailablePath.Point);
                }
            }
        }

        public void SetDamageAfterShot()
        {
            //Доп.проверка на случай убийства моба
            if (MobTarget.CurrentValue == null) return;
            var shot = new ShotData
            {
                Damage = _warriorEntity.Damage.Value,
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