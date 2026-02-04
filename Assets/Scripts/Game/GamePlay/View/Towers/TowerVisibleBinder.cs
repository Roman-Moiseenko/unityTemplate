using System;
using System.Collections.Generic;
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
        private TowerAttackViewModel _viewModel;
        private float _minDistance; // => _viewModel.MinDistance;
        private IDisposable _disposable;

        private void Awake()
        {
            //По умолчанию коллайдер отключен
            visibleCollider.gameObject.SetActive(false);
            Physics.reuseCollisionCallbacks = true;
        }

        public void Bind(TowerAttackViewModel viewModel)
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

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Mob")) return; //Обрабатываем только мобов
            var mobBinder = other.gameObject.GetComponent<MobBinder>();
            if (mobBinder.ViewModel.IsDead.CurrentValue) return; //Лаг задержки удаления модели
            _viewModel.PullTargets.Add(mobBinder.ViewModel); //Добавляем моба в пулл целей
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Mob")) return; //Обрабатываем только мобов
            if (_viewModel.PullTargets.Count != 0)
            {
                //Когда моб выходит из зоны видимости, удаляем из Пула
                var mobBinder = other.gameObject.GetComponent<MobBinder>();
                if (mobBinder.ViewModel.IsDead.CurrentValue) return; //Лаг задержки удаления модели
                _viewModel.PullTargets.Remove(mobBinder.ViewModel);
            }
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}