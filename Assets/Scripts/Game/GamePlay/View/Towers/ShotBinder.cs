using System;
using System.Collections;
using System.Collections.Generic;
using Game.Common;
using Game.GamePlay.View.Mobs;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class ShotBinder : MonoBehaviour
    {
        protected TowerAttackViewModel _viewModel;
        protected IDisposable _disposable;
        protected float _baseYMissile = 0f;
        protected ReactiveProperty<Vector3> _target = new();
        protected readonly ReactiveProperty<bool> _isMoving = new(false);
        protected MobViewModel _mobViewModel;
        protected IDisposable _targetSubscription;

        public List<string> LoggerShot = new();

        public void Bind(TowerAttackViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            transform.gameObject.SetActive(false);
            _baseYMissile = transform.position.y;

            viewModel.MobTargets.ObserveRemove().Subscribe(_ =>
            {
                if (viewModel.MobTargets.Count == 0) StopShot(null);
            }).AddTo(ref d);
            _disposable = d.Build();
        }

        public virtual void FirePrepare(MobViewModel mobViewModel)
        {
            _mobViewModel = mobViewModel;
            LoggerShot.Clear();

            // Отписываемся от предыдущей подписки, если была
            _targetSubscription?.Dispose();
            _targetSubscription = null;

            // Копируем значение, а не ссылку на ReactiveProperty из другой ViewModel
            _target.Value = mobViewModel.PositionTargetForShot.Value;
            LoggerShot.Add(mobViewModel.ConfigId);
            LoggerShot.Add(_target.Value.ToString());

            // Подписываемся на обновление позиции моба (с проверкой на disposed)
            try
            {
                _targetSubscription = mobViewModel.PositionTargetForShot
                    .Where(_ => _isMoving.CurrentValue)
                    .Subscribe(v =>
                    {
                        _target.Value = v;
                        LoggerShot.Add("_target move " + _target.Value.ToString());
                    });
            }
            catch (ObjectDisposedException)
            {
                LoggerShot.Add("Target already disposed");
            }

            transform.localPosition = new Vector3(0, _baseYMissile, 0);
        }

        public virtual IEnumerator FireStart()
        {
            transform.gameObject.SetActive(true);
            _isMoving.OnNext(true);

            while (_isMoving.Value)
            {
                var speedEntity = _viewModel.SpeedShot * AppConstants.SHOT_BASE_SPEED;
                yield return null;
                transform.position = Vector3.MoveTowards(transform.position, _target.CurrentValue,
                    Time.deltaTime * speedEntity);

                if (Vector3.Distance(transform.position, _target.CurrentValue) < 0.01)
                {
                    LoggerShot.Add("Distance < 0.01 " + _target.CurrentValue);
                    StopShot(_target.CurrentValue);
                    yield break;
                }

                yield return null;
            }
        }

        public virtual void StopShot(Vector3? position)
        {
            transform.gameObject.SetActive(false);
            _isMoving.OnNext(false);
            if (_viewModel.IsSingleTarget)
            {
                if (_mobViewModel != null) _viewModel.SetDamageAfterShot(_mobViewModel);
                LoggerShot.Add("Set Damage Single");
            }
            else if (position != null)
            {
                SetDamageByCollider((Vector3)position);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Mob")) return;
            LoggerShot.Add("OnTriggerEnter");
            StopShot(other.transform.position);
        }

        private void SetDamageByCollider(Vector3 position)
        {
            var colliders = Physics.OverlapSphere(position, 0.5f);
            foreach (var colliderTarget in colliders)
            {
                if (colliderTarget.gameObject.CompareTag("Mob"))
                {
                    var mob = colliderTarget.gameObject.GetComponent<MobBinder>();
                    if (_viewModel.IsTargetForDamage(mob.ViewModel.IsFly))
                        _viewModel.SetDamageAfterShot(mob.ViewModel);
                }
            }

            LoggerShot.Add("Set Damage By Collider");
        }

        private void OnDestroy()
        {
            _targetSubscription?.Dispose();
            _disposable?.Dispose();
        }
    }
}