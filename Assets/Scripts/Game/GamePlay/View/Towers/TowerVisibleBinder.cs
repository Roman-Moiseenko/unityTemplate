using System;
using System.Diagnostics;
using Game.GamePlay.View.Mobs;
using R3;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Game.GamePlay.View.Towers
{
    public class TowerVisibleBinder : MonoBehaviour
    {

        [SerializeField] private SphereCollider visibleCollider;
        private TowerViewModel _viewModel;
        private float _minDistance; // => _viewModel.MinDistance;
        private IDisposable _disposable;

        private void Awake()
        {
            //По умолчанию коллайдер отключен
            visibleCollider.gameObject.SetActive(false); 
        }

        public void Bind(TowerViewModel viewModel)
        {
            visibleCollider.gameObject.SetActive(true);
            var d = Disposable.CreateBuilder();
            _minDistance = viewModel.MinDistance;
            _viewModel = viewModel;
            _viewModel.MaxDistance.Subscribe(v =>
            {
                visibleCollider.radius = 0.5f + v;
            }).AddTo(ref d);

            _disposable = d.Build();
        }


        private void OnCollisionEnter(Collision other)
        {
            CheckAndAddToTarget(other);
        }

        private void OnCollisionStay(Collision other)
        {
            CheckAndAddToTarget(other);
        }

        private void CheckAndAddToTarget(Collision other)
        {
            if (!other.gameObject.CompareTag("Mob")) return; //Обрабатываем только мобов
            //Если мультишот или нет целей, то при движении цели
            if (_viewModel.TowerEntity.IsMultiShot || _viewModel.MobTargets.Count == 0)
            {
                var mobBinder = other.gameObject.GetComponent<MobBinder>();
                if (mobBinder.ViewModel.IsDead.CurrentValue) return; //Лаг задержки удаления модели
                
                if (_viewModel.MinDistance != 0f) //У башни есть минимальная дистанция
                {
                    var distance = Vector3.Distance(mobBinder.ViewModel.PositionTarget.CurrentValue, _viewModel.PositionMap.CurrentValue);
                    if (distance - 0.5f < _viewModel.MinDistance) return;
                }
                
                _viewModel.SetTarget(mobBinder.ViewModel);
                //Debug.Log($"Mob {mobBinder.ViewModel.UniqueId} Enter {_viewModel.UniqueId}");
                //  Debug.Log("Mob Enter " + mobBinder.ViewModel.UniqueId);
            }
            
        }
        
        private void OnCollisionExit(Collision other)
        {
            if (!other.gameObject.CompareTag("Mob")) return; //Обрабатываем только мобов
            if (_viewModel.MobTargets.Count != 0)
            {
                var mobBinder = other.gameObject.GetComponent<MobBinder>();
                Debug.Log($"Mob {mobBinder.ViewModel.UniqueId} Exit {_viewModel.UniqueId}");
                _viewModel.RemoveTarget(mobBinder.ViewModel);
            }
        }
        

        private void OnDestroy()
        {
            _disposable?.Dispose();
            
        }
    }
}