using System;
using System.Collections;
using System.Collections.Generic;
using Game.GamePlay.View.Mobs;
using Game.GamePlay.View.Warriors;
using MVVM.Storage;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerBasePlacementBinder : TowerBaseBinder<TowerPlacementViewModel>
    {
        [SerializeField] private TowerPlacementVisibleBinder visibleBinder;
        private ObservableDictionary<int, WarriorBinder> warriors = new();


        private IDisposable _disposable;
        protected override void OnBind(TowerPlacementViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            visibleBinder.Bind(viewModel);
            foreach (var warriorViewModel in viewModel.Warriors)
                CreateWarrior(warriorViewModel);
            
            //При добавлении вью-модели создаем Монобех Warrior, и запускаем его ч/з Bind()
            viewModel.Warriors.ObserveAdd().Subscribe(e =>
            {
                var warriorViewModel = e.Value;
                CreateWarrior(warriorViewModel);
            }).AddTo(ref d);
            //При удалении модели запускаем анимацию и подписываемся на ее завершение, после удаляем из списка Binders
            viewModel.Warriors.ObserveRemove().Subscribe(e =>
            {
                var warriorViewModel = e.Value;
                var warriorBinder = warriors[warriorViewModel.UniqueId];
                warriorBinder.StartDeadAnimation()
                    .Where(x => x)
                    .Subscribe(_ => warriors.Remove(warriorViewModel.UniqueId));
            }).AddTo(ref d);

            //Отдельно на удаление Binder, т.к. возможно будут удалятся в другом месте (при удалении башни и т.п.) 
            warriors.ObserveRemove().Subscribe(e =>
            {
                var warriorBinder = e.Value.Value;
                Destroy(warriorBinder.gameObject);
                Destroy(warriorBinder);
            }).AddTo(ref d);

            _disposable = d.Build();
            //MainCoroutine = StartCoroutine(PlacementUpdateTower());
        }
        
        protected override void RestartAfterUpdate()
        {
            //TODO Обновляем внешний вид воинов и запускаем эффекты
            
        }
        
        private void CreateWarrior(WarriorViewModel warriorViewModel)
        {
            var prefabWarriorPath =
                $"Prefabs/Gameplay/Warriors/Warrior-{warriorViewModel.ConfigId}"; //Перенести в настройки уровня
            var warriorPrefab = Resources.Load<WarriorBinder>(prefabWarriorPath);
            var createdWarrior = Instantiate(warriorPrefab, transform);
            createdWarrior.Bind(warriorViewModel);
            
            warriors.Add(warriorViewModel.UniqueId, createdWarrior);
        }

        protected override void OnAfterDestroy()
        {
            ViewModel.Dispose();
            _disposable?.Dispose();
        }
    }
}