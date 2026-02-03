using System;
using System.Collections.Generic;
using System.Diagnostics;
using Game.GamePlay.View.Mobs;
using R3;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Game.GamePlay.View.Towers
{
    public class TowerUnVisibleBinder : MonoBehaviour
    {
        [SerializeField] private SphereCollider visibleCollider;
        private TowerViewModel _viewModel;
        private IDisposable _disposable;

        private void Awake()
        {
            //По умолчанию коллайдер отключен
            visibleCollider.gameObject.SetActive(false);
            Physics.reuseCollisionCallbacks = true;
        }

        public void Bind(TowerViewModel viewModel)
        {
            visibleCollider.gameObject.SetActive(true);
            var d = Disposable.CreateBuilder();
            visibleCollider.radius = viewModel.MinDistance + 0.5f;
            _viewModel = viewModel;
            _disposable = d.Build();
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Mob")) return; //Обрабатываем только мобов
            var mobBinder = other.gameObject.GetComponent<MobBinder>();
            if (mobBinder.ViewModel.IsDead.CurrentValue) return; //Лаг задержки удаления модели
            _viewModel.PullTargets.Remove(mobBinder.ViewModel); //Удаляем моба из пулла
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Mob")) return; //Обрабатываем только мобов

            //Когда моб выходит из зоны видимости, удаляем из Пула
            var mobBinder = other.gameObject.GetComponent<MobBinder>();
            if (mobBinder.ViewModel.IsDead.CurrentValue) return; //Лаг задержки удаления модели
            _viewModel.PullTargets.Add(mobBinder.ViewModel); //Добавляем моба в пулл целей
        }
        
        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}