using System;
using System.Collections.Generic;
using System.Diagnostics;
using Game.GamePlay.View.Mobs;
using R3;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Game.GamePlay.View.Towers
{
    public class TowerPlacementVisibleBinder : MonoBehaviour
    {

        private TowerPlacementViewModel _viewModel;
        private float _minDistance; // => _viewModel.MinDistance;
        private IDisposable _disposable;

        private void Awake()
        {
            //По умолчанию коллайдер отключен
            transform.gameObject.SetActive(false);
        }

        public void Bind(TowerPlacementViewModel viewModel)
        {
            _viewModel = viewModel;
            transform.gameObject.SetActive(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Mob")) return; //Обрабатываем только мобов
            var mobBinder = other.gameObject.GetComponent<MobBinder>();
            if (_viewModel.IsWay != mobBinder.ViewModel.IsWay) return; //Разные пути 
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