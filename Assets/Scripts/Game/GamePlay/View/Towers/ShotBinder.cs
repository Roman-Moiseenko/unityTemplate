using System;
using System.Collections;
using Game.Common;
using Game.GamePlay.View.Mobs;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class ShotBinder : MonoBehaviour
    {
        protected TowerViewModel _viewModel;
        protected IDisposable _disposable;
        protected float _baseYMissile = 0f;
        protected ReactiveProperty<Vector3> _target = new();
        protected readonly ReactiveProperty<bool> _isMoving = new(false);
        protected MobViewModel _mobViewModel;
        public void Bind(TowerViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            transform.gameObject.SetActive(false);
            _baseYMissile = transform.position.y;
            
            viewModel.MobTargets.ObserveRemove().Subscribe(_ =>
            {
                if (viewModel.MobTargets.Count == 0) StopShot();
            }).AddTo(ref d);
            _disposable = d.Build();
        }

        public virtual void FirePrepare(MobViewModel mobViewModel)
        {
            _mobViewModel = mobViewModel;
            _target = mobViewModel.PositionTarget;
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
                transform.position = Vector3.MoveTowards(transform.position, _target.CurrentValue,  Time.deltaTime * speedEntity);

                if (Vector3.Distance(transform.position, _target.CurrentValue) < 0.1) StopShot();
                if (_target == null || _target.Value == null) StopShot();
                yield return null;
            }
        }
        
        public virtual void StopShot()
        {
            transform.gameObject.SetActive(false);
            _isMoving.OnNext(false);
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag("Mob")) return;
            StopShot();
            _viewModel.SetDamageAfterShot(_mobViewModel);
        }
        

    }
}