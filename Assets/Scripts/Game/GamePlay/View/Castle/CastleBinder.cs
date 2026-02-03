using System;
using System.Collections;
using Game.GamePlay.View.Mobs;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Castle
{
    public class CastleBinder : MonoBehaviour
    {
        [SerializeField] private CastleGunBinder gunLeft;
        [SerializeField] private CastleGunBinder gunCenter;
        [SerializeField] private CastleGunBinder gunRight;
        [SerializeField] private ParticleSystem explosionEffect;
        [SerializeField] private CastleVisibleBinder visibleBinder;

        private IDisposable _disposable;
        private CastleViewModel _viewModel;
        private Coroutine _coroutine;
        private Coroutine _mainCoroutine;
        
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

            _mainCoroutine = StartCoroutine(FireUpdateCastle());
        }

        private IEnumerator FireUpdateCastle()
        {
            while (true)
            {
                yield return null;
                //Обходим все цели в модели
                if (_viewModel.MobTarget.CurrentValue == null) continue;
                gunLeft.Fire(_viewModel.MobTarget.CurrentValue);
                gunCenter.Fire(_viewModel.MobTarget.CurrentValue);
                gunRight.Fire(_viewModel.MobTarget.CurrentValue);
                yield return new WaitForSeconds(_viewModel.Speed);
            }
        }
        
        private void OnDestroy()
        {
            StopCoroutine(_mainCoroutine);
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