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
            _target = mobViewModel.PositionTargetForShot; //Скорректирована высота для Fly, Boss и остальные

            LoggerShot.Add(mobViewModel.ConfigId);
            LoggerShot.Add(_target.Value.ToString());
            _target.Subscribe(v => LoggerShot.Add("_target move " + _target.Value.ToString()));

            transform.localPosition = new Vector3(0, _baseYMissile, 0); //Размещение снаряда в точке 0
        }

        public virtual IEnumerator FireStart()
        {
            transform.gameObject.SetActive(true);
            _isMoving.OnNext(true);

            while (_isMoving.Value)
            {
                var speedEntity = _viewModel.SpeedShot * AppConstants.SHOT_BASE_SPEED;
                yield return null;
                //TODO Расчет координат полета от типа снаряда
                transform.position = Vector3.MoveTowards(transform.position, _target.CurrentValue,
                    Time.deltaTime * speedEntity);

                if (_target == null || _target.Value == null)
                {
                    LoggerShot.Add("_target is null");
                    StopShot(transform.position);
                    yield break;
                }

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
                if (_target != null) _viewModel.SetDamageAfterShot(_mobViewModel); //Наносим урон одной цели
                LoggerShot.Add("Set Damage Single");
            }
            else if (position != null)
            {
                SetDamageByCollider((Vector3)position);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
//            Debug.Log(other.gameObject.tag + " " + _target.Value );
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
                    //Проверяем на Air/Ground и наносим урон
                    if (_viewModel.IsTargetForDamage(mob.ViewModel.IsFly))
                        _viewModel.SetDamageAfterShot(mob.ViewModel);
                }
            }

            LoggerShot.Add("Set Damage By Collider");
        }


        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}