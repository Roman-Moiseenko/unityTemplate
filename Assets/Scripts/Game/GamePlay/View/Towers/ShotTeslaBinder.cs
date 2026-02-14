using System;
using System.Collections;
using Game.GamePlay.View.Mobs;
using Game.State.Maps.Mobs;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class ShotTeslaBinder : ShotBinder
    {
        private IDisposable _disposableTesla;
        public override void FirePrepare(MobViewModel mobViewModel)
        {
            _mobViewModel = mobViewModel;
            transform.gameObject.SetActive(true);
            _disposableTesla?.Dispose();
            _disposableTesla = mobViewModel.PositionTargetForShot.Subscribe(position =>
            {
                var scale = Vector3.Distance(transform.position, position) / 0.5f;
                transform.localScale = new Vector3(1, 1, scale);
                var toDirection = position - transform.position;
                var fromDirection = new Vector3(0, 0, 1f);
                transform.rotation = Quaternion.FromToRotation(fromDirection, toDirection);
            });
        }

        public override IEnumerator FireStart()
        {
            yield return new WaitForSeconds(_viewModel.Speed);
            transform.gameObject.SetActive(false);
        }
        
        private void OnDestroy()
        {
            _disposableTesla?.Dispose();
            _disposable?.Dispose();
            
        }
        public override void StopShot()
        {
            base.StopShot();
            _disposableTesla?.Dispose();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Mob")) return;
            _viewModel.SetDamageAfterShot(_mobViewModel);
        }

    }
}