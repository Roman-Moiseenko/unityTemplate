using System;
using System.Collections.Generic;
using System.Linq;
using Game.MainMenu.View.ScreenInventory.PopupBlacksmith.PrefabBinders;
using Game.State.Inventory.TowerCards;
using MVVM.UI;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenInventory.PopupBlacksmith
{
    public class PopupBlacksmithBinder : PopupBinder<PopupBlacksmithViewModel>
    {
        [SerializeField] private Transform upgradedInfo;
        [SerializeField] private Transform upgradedTower;
        [SerializeField] private Transform upgradingTower;

        [SerializeField] private Transform upgradingNecessary1;
        [SerializeField] private Transform upgradingNecessary2;

        [SerializeField] private Transform containerResources;

        [SerializeField] private Button buttonMerge;

        private readonly List<TowerCardResourceBinder> _createdTowerCardMap = new();

        private IDisposable _disposable;

        protected override void OnBind(PopupBlacksmithViewModel viewModel)
        {
            base.OnBind(viewModel);
            var d = Disposable.CreateBuilder();

            foreach (var towerCard in viewModel.TowerCardMaps)
            {
                CreateTowerCard(towerCard);
            }

            UpdateHeightContainerResource();
            viewModel.TowerCardMaps.ObserveAdd().Subscribe(e =>
            {
                CreateTowerCard(e.Value);
                UpdateHeightContainerResource();
            }).AddTo(ref d);
            viewModel.TowerCardMaps.ObserveClear().Subscribe(_ =>
            {
                foreach (var createdTower in _createdTowerCardMap)
                {
                    Destroy(createdTower.gameObject);
                }

                _createdTowerCardMap.Clear();
                UpdateHeightContainerResource();
            }).AddTo(ref d);

            upgradingTower.GetComponent<TowerCardUpgradingBinder>().Bind(viewModel.TowerUpgrading);
            upgradingNecessary1.Find("TowerCardUpgrading").GetComponent<TowerCardUpgradingBinder>()
                .Bind(viewModel.UpgradingNecessary1);
            upgradingNecessary2.Find("TowerCardUpgrading").GetComponent<TowerCardUpgradingBinder>()
                .Bind(viewModel.UpgradingNecessary2);
            upgradedInfo.GetComponent<InfoUpgradedBinder>().Empty();
            
            var upgradedTowerBinder = upgradedTower.GetComponent<TowerCardUpgradedBinder>();
            viewModel.TowerUpgrading.IsSetCard.Subscribe(v =>
            {
                if (v)
                {
                    upgradedTowerBinder.Bind(viewModel.TowerUpgrading);
                    upgradedInfo.GetComponent<InfoUpgradedBinder>().Bind(viewModel.GetInfoUpdates());

                    //InfoUpgradedBinder.Show(viewModel.TowerUpgrading);
                    //Показываем характеристики
                    upgradedTower.gameObject.SetActive(true);
                    //Не показываем другого типа 
                    foreach (var towerCardResourceBinder in _createdTowerCardMap)
                    {
                        if (towerCardResourceBinder.ConfigId != viewModel.TowerUpgrading.ConfigId || 
                            towerCardResourceBinder.EpicLevel != viewModel.TowerUpgrading.EpicLevel
                            )
                            towerCardResourceBinder.gameObject.SetActive(false);
                    }
                }
                else
                {
                    upgradedTower.gameObject.SetActive(false);
                    upgradedInfo.GetComponent<InfoUpgradedBinder>().Empty();
                    //InfoUpgradedBinder.Empty();
                    foreach (var towerCardResourceBinder in _createdTowerCardMap)
                    {
                        towerCardResourceBinder.gameObject.SetActive(true);
                    }
                }

                UpdateHeightContainerResource();
            }).AddTo(ref d);

            viewModel.UpgradingNecessary1.IsSetCard
                .Merge(viewModel.UpgradingNecessary2.IsSetCard)
                .Subscribe(v =>
                {
                  /*  var l0 = viewModel.TowerUpgrading.Level;
                    var l1 = viewModel.UpgradingNecessary1.Level;
                    var l2 = viewModel.UpgradingNecessary2.Level;*/

                    upgradedTowerBinder.SetLevel(viewModel.MaxLevel.CurrentValue);
                    if (viewModel.TowerUpgrading.IsSetCard.CurrentValue)
                        upgradedInfo.GetComponent<InfoUpgradedBinder>().Bind(viewModel.GetInfoUpdates());    
                    
                    buttonMerge.interactable = viewModel.TowerUpgrading.IsSetCard.CurrentValue &&
                                               viewModel.UpgradingNecessary1.IsSetCard.CurrentValue &&
                                               viewModel.UpgradingNecessary2.IsSetCard.CurrentValue;
                })
                .AddTo(ref d);

            _disposable = d.Build();
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
            var count = 0;
            foreach (var towerCardResourceBinder in _createdTowerCardMap)
            {
                if (towerCardResourceBinder.gameObject.activeSelf)
                {
                    count++;
                }
            }


            const int blockHeight = 250;
            const int blockSpacing = 22;
            var rows = Math.Ceiling(count / 5f); // container.childCount

            container.sizeDelta = count == 0
                ? new Vector2(1040, 0)
                : new Vector2(1040, (float)(rows * blockHeight + (rows - 1) * blockSpacing));
        }

        private void OnMergeTowerCard()
        {
            ViewModel.MergeTowerCard();
        }

        private void OnEnable()
        {
            buttonMerge.onClick.AddListener(OnMergeTowerCard);
        }

        private void OnDisable()
        {
            buttonMerge.onClick.RemoveListener(OnMergeTowerCard);
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            _disposable.Dispose();
        }
    }
}