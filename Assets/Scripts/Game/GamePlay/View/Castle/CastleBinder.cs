using System;
using System.Collections;
using System.Collections.Generic;
using Game.GamePlay.View.Mobs;
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
        [SerializeField] private CastleVisibleBinder visibleBinder;

        private IDisposable _disposable;
        private CastleViewModel _viewModel;
        private Coroutine _coroutine;

        public void Bind(CastleViewModel viewModel)
        {
            _viewModel = viewModel;
            visibleBinder.Bind(viewModel);
            gunLeft.Bind(viewModel);
            gunCenter.Bind(viewModel);
            gunRight.Bind(viewModel);
            var d = Disposable.CreateBuilder();
            transform.position = new Vector3(
                viewModel.Position.x,
                0,
                viewModel.Position.y
            );

            viewModel.MobTarget.Subscribe(mobViewModel =>
            {
                if (mobViewModel == null)
                { //На случай, если моба убьет не Замок или цель вышла из зоны поражения
                    StopFire();
                    return;
                }
                _coroutine = StartCoroutine(FireOneTarget(mobViewModel));
                //TODO Протестировать и придумать отписку после удаления моба
                mobViewModel.IsDead
                    .Where(x => x)
                    .Subscribe(_ => _viewModel.RemoveTarget(mobViewModel));
            }).AddTo(ref d);
            
            //Когда все выстрелы завершились(попали в цель)
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
            _disposable = d.Build();
        }

        private IEnumerator FireOneTarget(MobViewModel mobViewModel)
        {
            while (!mobViewModel.IsDead.CurrentValue)
            {
                gunLeft.Fire(mobViewModel);
                gunCenter.Fire(mobViewModel);
                gunRight.Fire(mobViewModel);
                yield return new WaitForSeconds(_viewModel.Speed);
            }
        }

        private void StopFire()
        {
            if(_coroutine != null) StopCoroutine(_coroutine);
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
            var mobViewModel = _viewModel.MobTarget.CurrentValue;
            if (mobViewModel == null) return;
            explosionEffect.transform.position = mobViewModel.PositionTarget.CurrentValue;
            explosionEffect.Play();
            _viewModel.SetDamageAfterShot();
        }
    }
}