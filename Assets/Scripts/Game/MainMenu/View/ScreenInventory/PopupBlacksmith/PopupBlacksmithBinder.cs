using System;
using System.Collections.Generic;
using Game.MainMenu.View.ScreenInventory.PopupBlacksmith.PrefabBinders;
using Game.State.Inventory.TowerCards;
using MVVM.UI;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.MainMenu.View.ScreenInventory.PopupBlacksmith
{
    public class PopupBlacksmithBinder  : PopupBinder<PopupBlacksmithViewModel>
    {
        [SerializeField] private Transform upgradedTower;
        [SerializeField] private Transform upgradingTower;

        [SerializeField] private Transform upgradingNecessary1;
        [SerializeField] private Transform upgradingNecessary2;
        
        [SerializeField] private Transform containerResources;

        private readonly List<TowerCardResourceBinder> _createdTowerCardMap = new();
        protected override void OnBind(PopupBlacksmithViewModel viewModel)
        {
            base.OnBind(viewModel);
            

            foreach (var towerCard in viewModel.TowerCardMaps)
            {
               // Debug.Log(towerCard.UniqueId);
                CreateTowerCard(towerCard);
            }

            UpdateHeightContainerResource();
            viewModel.TowerCardMaps.ObserveAdd().Subscribe(e =>
            {
               // Debug.Log("viewModel.TowerCardMaps.ObserveAdd => " + e.Value.UniqueId);
                CreateTowerCard(e.Value);
                UpdateHeightContainerResource();
            });
            
            viewModel.TowerCardMaps.ObserveClear().Subscribe(_ =>
            {
                foreach (var createdTower in _createdTowerCardMap)
                {
                    Destroy(createdTower.gameObject);
                }
                _createdTowerCardMap.Clear();
                UpdateHeightContainerResource();
            });
            
            upgradingTower.GetComponent<TowerCardUpgradedBinder>().Bind(viewModel.TowerUpgrading);
            upgradingNecessary1.Find("TowerCardUpgrading").GetComponent<TowerCardUpgradedBinder>().Bind(viewModel.UpgradingNecessary1);
            upgradingNecessary2.Find("TowerCardUpgrading").GetComponent<TowerCardUpgradedBinder>().Bind(viewModel.UpgradingNecessary2);
        }
        
        
        private void CreateTowerCard(TowerCardResourceViewModel viewModel)
        {
            var prefabTowerCardPath =
                $"Prefabs/UI/MainMenu/ScreenInventory/Popups/TowerCardResource";
            var towerPrefab = Resources.Load<TowerCardResourceBinder>(prefabTowerCardPath);
            var createdTower = Instantiate(towerPrefab, containerResources);
            createdTower.Bind(viewModel);
            _createdTowerCardMap.Add(createdTower);
        }
        
        private void UpdateHeightContainerResource()
        {
            var container = containerResources.GetComponent<RectTransform>();
            const int blockHeight = 250;
            const int blockSpacing = 22;
            var rows = Math.Ceiling(container.childCount / 5f);
            
            container.sizeDelta = container.childCount == 0 
                ? new Vector2(1040, 0) 
                : new Vector2(1040, (float)(rows * blockHeight + (rows - 1) * blockSpacing));
        }
    }
}