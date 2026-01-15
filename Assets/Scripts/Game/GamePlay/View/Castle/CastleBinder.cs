using System;
using System.Collections;
using System.Collections.Generic;
using Game.State.Maps.Mobs;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Castle
{
    public class CastleBinder : MonoBehaviour
    {
        //[SerializeField] private CastleShotsBinder shots;
        [SerializeField] private CastleGunBinder gunLeft;
        [SerializeField] private CastleGunBinder gunCenter;
        [SerializeField] private CastleGunBinder gunRight;
        [SerializeField] private ParticleSystem explosionEffect;


        private IDisposable _disposable;

        private CastleViewModel _viewModel;

        //private readonly Dictionary<MobEntity, IDisposable> _targetsDisposables = new();
        private Coroutine _coroutine;

        public void Bind(CastleViewModel viewModel)
        {
            _viewModel = viewModel;
            gunLeft.Bind(viewModel);
            gunCenter.Bind(viewModel);
            gunRight.Bind(viewModel);
            var d = Disposable.CreateBuilder();
            transform.position = new Vector3(
                viewModel.Position.x,
                0,
                viewModel.Position.y
            );

            //shots.Bind(viewModel);
            viewModel.Target
                .ObserveAdd()
                .Subscribe(e =>
                {
                    var mobEntity = e.Value;
                    Fire(mobEntity);
                    //_coroutine = StartCoroutine(CastleFire(mobEntity));
                })
                .AddTo(ref d);

            //Когда все выстрелы завершились
            gunLeft.IsShotComplete
                .Merge(gunCenter.IsShotComplete)
                .Merge(gunRight.IsShotComplete)
                .Subscribe(v =>
                {
                    if (gunLeft.IsShotComplete.CurrentValue
                        && gunCenter.IsShotComplete.CurrentValue
                        && gunRight.IsShotComplete.CurrentValue)
                    {
                        FinishFire();
                    }
                }).AddTo(ref d);
            //Когда моба убъет другая башня
            viewModel.Target.ObserveRemove().Subscribe(e =>
            {
                //var mobEntity = e.Value;
                StopFire();
                // StopCoroutine(_coroutine);
                //  shots.StopShot();
            }).AddTo(ref d);
            _disposable = d.Build();
        }

        private void Fire(MobEntity mobEntity)
        {
            gunLeft.Fire(mobEntity);
            gunCenter.Fire(mobEntity);
            gunRight.Fire(mobEntity);
        }

        private void StopFire()
        {
            gunLeft.StopFire();
            gunCenter.StopFire();
            gunRight.StopFire();
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }

        private void FinishFire()
        {
            if (_viewModel.Target.Count == 0) return;
            var target = _viewModel.Target[0];
            explosionEffect.transform.position = target.PositionTarget.CurrentValue;
            explosionEffect.Play();
            _viewModel.CastleEntity.ClearTarget(); //удаляем цель из башни
        }
    }
}