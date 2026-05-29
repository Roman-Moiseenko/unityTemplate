using System;
using System.Collections;
using System.Collections.Generic;
using Game.Common;
using Game.GamePlay.Services;
using Game.State.Common;
using Game.State.Gameplay;
using Game.State.Maps.Mobs;
using Game.State.Maps.Roads;
using ObservableCollections;
using R3;
using Scripts.Utils;
using UnityEngine;

namespace Game.GamePlay.View.Mobs
{
    public class MobViewModel : IDisposable
    {
        private readonly MobEntity _mobEntity;

        public readonly ObservableList<IHasHeathViewModel> PullTargets = new();
        private readonly ReactiveProperty<IHasHeathViewModel> _target = new(null); //Текущая цель

        public int UniqueId => _mobEntity.UniqueId;
        public bool IsFly => _mobEntity.IsFly;
        public string ConfigId => _mobEntity.ConfigId;
        public readonly ReactiveProperty<bool> IsMoving = new(false);
        public readonly ReactiveProperty<bool> IsAttack = new(false);
        public ReactiveProperty<Vector2> Position => _mobEntity.Position;

        public Vector2 StartPosition;
        public Vector2Int StartDirection;

        public readonly List<RoadPoint> RoadPoints;

        //public ReactiveProperty<MobState> State; //TODO Возможно удалить или модифицировать до FSM
        public ReadOnlyReactiveProperty<float> CurrentHealth => _mobEntity.Health;
        public readonly float MaxHealth;

        public readonly ReactiveProperty<bool> FinishCurrentAnimation = new(true);
        public readonly ReactiveProperty<bool> AnimationDelete = new(false);
        public IReadOnlyObservableDictionary<string, MobDebuff> Debuffs => _mobEntity.Debuffs;
        public int Level => _mobEntity.Level;
        private float Damage => _mobEntity.Damage;

        public ReactiveProperty<Vector3> PositionTarget => _mobEntity.PositionTarget;
        public readonly ReactiveProperty<Vector3> PositionTargetForShot = new();
        public ReadOnlyReactiveProperty<bool> IsDead => _mobEntity.IsDead;
        public TypeDefence Defence => _mobEntity.Defence;
        public bool IsWay => _mobEntity.IsWay;

        public bool IsBoss => _mobEntity.IsBoss;
        private Coroutine _attackCoroutine;
        //private readonly GameplayStateProxy _gameplayState;
        private readonly Coroutines _coroutines;

        private readonly Dictionary<int, IDisposable> _pullTargetsDisposables = new();
        private DisposableBag _disposables = new();
        
        public MobViewModel(
            MobEntity mobEntity,
            GameplayStateProxy gameplayState,
            WaveService waveService
        )
        {
            //TODO Перенести в Binder как у Warriors, возможно через FSM
            _coroutines = GameObject.Find(AppConstants.COROUTINES).GetComponent<Coroutines>();

           // _gameplayState = gameplayState;
            _mobEntity = mobEntity;
            //CurrentHealth = mobEntity.Health;
            MaxHealth = mobEntity.Health.CurrentValue;

            
            //Моб вышел на дорогу, просчитываем путь и начальные координаты, от расположения ворот
            
            //Начальные координаты и направление для моба в зависимости от его пути
            Vector2 position;
            Vector2Int direction;

            if (mobEntity.IsWay)
            {
                position = waveService.GateWaveViewModel.Position.CurrentValue;
                direction = -1 * waveService.GateWaveViewModel.Direction.CurrentValue;
            }
            else
            {
                position = waveService.GateWaveSecondViewModel.Position.CurrentValue;
                direction = -1 * waveService.GateWaveSecondViewModel.Direction.CurrentValue;
            }
            mobEntity.SetStartPosition(position, direction);
            
            //Устанавливаем центр по высоте для ShotViewModel
            PositionTarget.Subscribe(v =>
            {
                var h = 0.15f; 
                if (IsFly)
                {
                    h = 0.7f;
                } 
                else if(_mobEntity.IsBoss)
                {
                    h = 0.3f;
                }
                PositionTargetForShot.Value = new Vector3(v.x, h, v.z);
            }).AddTo(ref _disposables);
            
            StartPosition = mobEntity.Position.CurrentValue;
            StartDirection = mobEntity.Direction.CurrentValue;
            RoadPoints = waveService.GenerateRoadPoints(mobEntity);

            IsMoving.Value = true;

            PullTargets.ObserveAdd().Subscribe(e =>
            {
              //  Debug.Log("Добавили в пул " + e.Value.GetType());
                var targetViewModel = e.Value;
                
                // Защита: подписываемся на IsDead только если он ещё не задиспожен
                IDisposable disposable = null;
                try
                {
                    disposable = targetViewModel.IsDead.Where(x => x).Subscribe(_ => PullTargets.Remove(targetViewModel));
                }
                catch (ObjectDisposedException)
                {
                    // Цель уже задиспожена, игнорируем
                    return;
                }

                if (_target.CurrentValue == null)
                {
                    _target.OnNext(targetViewModel);
                    IsMoving.OnNext(false);
                    IsAttack.OnNext(true);
                }

                _pullTargetsDisposables.Add(targetViewModel.UniqueId, disposable);
            }).AddTo(ref _disposables);

            PullTargets.ObserveRemove().Subscribe(e =>
            {
                var warrior = e.Value;
                if (_target.CurrentValue != null && warrior.UniqueId == _target.CurrentValue.UniqueId)
                {
                    _target.OnNext(null);
                }

                if (_pullTargetsDisposables.TryGetValue(warrior.UniqueId, out var disposable))
                {
                    disposable?.Dispose();
                    _pullTargetsDisposables.Remove(warrior.UniqueId);
                }

                if (PullTargets.Count == 0)
                {
                    IsMoving.Value = true;
                    IsAttack.Value = false;
                    _target.Value = null;
                }
            }).AddTo(ref _disposables);

            _target.Skip(1).Subscribe(target =>
            {
                if (target == null)
                {
                    foreach (var targetViewModel in PullTargets)
                    {
                        _target.OnNext(targetViewModel);
                        IsMoving.Value = false;
                        IsAttack.Value = true;
                        return;
                    }

                    IsMoving.Value = true;
                    IsAttack.Value = false;
                    return;
                }
                if (_coroutines != null)
                    _attackCoroutine = _coroutines.StartCoroutine(AttackTarget());
            }).AddTo(ref _disposables);
        }

        public IEnumerator WaitFinishAnimation()
        {
            while (!FinishCurrentAnimation.Value)
            {
                yield return null;
            }
        }

        public IEnumerator TimerDebuff(string configId, MobDebuff debuff)
        {
            //Пауза
            yield return new WaitForSeconds(debuff.Time);
            _mobEntity.RemoveDebuff(configId);
        }

        public float GetSpeedMob()
        {
            return _mobEntity.Speed();
        }

      /*  public void SetDamage(float damage)
        {
            _mobEntity.SetDamage(damage);
        }
        */
        public Vector3 GetTargetPosition(int index)
        {
            var newValue = RoadPoints[index].Point; //_currentIndexListPoint
            return new Vector3(newValue.x, 0.0f, newValue.y);
        }

        public void StartAnimationDelete()
        {
            FinishCurrentAnimation.Value = false;
            AnimationDelete.Value = true;
        }

        public void RemoveDebuff(string configId)
        {
            _mobEntity.RemoveDebuff(configId);
        }
        
        public IEnumerator AttackTarget()
        {
//            Debug.Log(" AttackTarget " + Target.CurrentValue.UniqueId);
            if (_mobEntity.IsDead.CurrentValue) yield break;
            _target.CurrentValue.DamageReceived(Damage, _mobEntity.Defence);
            yield return new WaitForSeconds(_mobEntity.SpeedAttack / AppConstants.MOB_SPEED_ATTACK);

            if (_target.CurrentValue == null) yield break;
            if (!_target.CurrentValue.IsDead.CurrentValue && _coroutines != null) 
                _attackCoroutine = _coroutines.StartCoroutine(AttackTarget());
        }
        public void StopAttack()
        {
            if (_attackCoroutine != null)
            {
                // Проверка, что объект Coroutines всё ещё существует (не уничтожен при выгрузке сцены)
                try
                {
                    if (_coroutines != null)
                        _coroutines.StopCoroutine(_attackCoroutine);
                }
                catch (MissingReferenceException)
                {
                    // Объект [COROUTINES] уже уничтожен, корутина остановится сама
                }
                _attackCoroutine = null;
            }
        }
        public void Dispose()
        {
            try
            {
                if (_coroutines != null)
                    StopAttack();
            }
            catch (MissingReferenceException)
            {
                // Coroutines уже уничтожен (при выгрузке сцены)
            }
            
            // Очищаем PullTargets до диспоуза подписок, чтобы избежать вызова колбэков
            PullTargets.Clear();
            
            IsMoving?.Dispose();
            IsAttack?.Dispose();
            FinishCurrentAnimation?.Dispose();
            AnimationDelete?.Dispose();
            PositionTargetForShot?.Dispose();
            _disposables.Dispose();
        }
    }
}