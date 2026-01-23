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
        // [SerializeField] protected Transform missile;
        
        
        public override void FirePrepare(MobViewModel mobViewModel)
        {
            var d = Disposable.CreateBuilder();
//            Debug.Log("ShotTeslaBinder " + mobViewModel.UniqueId + " " + missile.position);

            _mobViewModel = mobViewModel;
            transform.gameObject.SetActive(true);
            mobViewModel.PositionTarget.Subscribe(position =>
            {
                //Debug.Log($"PositionTarget {transform.position} / {position}");
                var dist = Vector3.Distance(transform.position, position);
                var scale = dist / 0.5f;
               // Debug.Log($"PositionTarget {transform.position} / {position} dist={dist}" );
                transform.localScale = new Vector3(1, 1, scale);
//                Debug.Log($"Для моба {mobEntity.UniqueId} установлена длина {dist}");
                var toDirection = position - transform.position;
                var fromDirection = new Vector3(0, 0, 1f);

                //TODO Менять длину коллайдера на dist
                transform.rotation = Quaternion.FromToRotation(fromDirection, toDirection);
            }).AddTo(ref d);
            _disposable = d.Build();
        }
      /*  
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.1f);
            Gizmos.DrawSphere(_mobViewModel.PositionTarget.CurrentValue, 0.1f);
            Gizmos.DrawLine(transform.position, _mobViewModel.PositionTarget.CurrentValue);
        }
        */
        public override IEnumerator FireStart()
        {
            yield return new WaitForSeconds(_viewModel.Speed);
            transform.gameObject.SetActive(false);
        }
        
        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
        public override void StopShot()
        {
            //
        }
        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag("Mob")) return;
            
            Debug.Log($"Выстрел {_viewModel.UniqueId} => {_mobViewModel.UniqueId} ");
            //Debug.Log(" кол-во целей" + _viewModel.MobTargets.Count);
            _viewModel.SetDamageAfterShot(_mobViewModel);
        }
/*
        private void OnCollisionExit(Collision other)
        {
            if (!other.gameObject.CompareTag("Mob")) return;
            transform.gameObject.SetActive(false);
        }
        */
    }
}