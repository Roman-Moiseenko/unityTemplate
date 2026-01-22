using System;
using Game.GamePlay.View.Mobs;
using Game.State.Maps.Mobs;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class ShotTeslaBinder : MonoBehaviour
    {
        [SerializeField] protected Transform missile;

        private IDisposable _disposable;
        public void Bind(MobViewModel mobViewModel)
        {
            var d = Disposable.CreateBuilder();
            
            missile.gameObject.SetActive(true);
            mobViewModel.PositionTarget.Subscribe(position =>
            {
                var dist = Vector3.Distance(transform.position, position) + 1f;
                missile.localScale = new Vector3(1, 1, dist);
//                Debug.Log($"Для моба {mobEntity.UniqueId} установлена длина {dist}");
                var toDirection = position - transform.position;
                var fromDirection = new Vector3(0, 0, 1f);

                transform.rotation = Quaternion.FromToRotation(fromDirection, toDirection);
            }).AddTo(ref d);
            _disposable = d.Build();
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}