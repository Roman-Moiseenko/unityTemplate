using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.MainMenu.Services;
using Game.MainMenu.View.ScreenInventory.PopupBlacksmith.PrefabBinders;
using Game.State;
using Game.State.Inventory;
using Game.State.Inventory.TowerCards;
using Game.State.Root;
using MVVM.UI;
using Newtonsoft.Json;
using NUnit.Framework;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.MainMenu.View.ScreenInventory.PopupBlacksmith
{
    public class PopupBlacksmithViewModel : WindowViewModel
    {
        private readonly GameStateProxy _gameState;
        private readonly TowerCardPlanService _towerCardPlanService;
        public override string Id => "PopupBlacksmith";
        public override string Path => "MainMenu/ScreenInventory/Popups/";

        public List<TowerCard> BaseListCard = new();
        public ObservableList<TowerCardResourceViewModel> TowerCardMaps = new();
        private readonly DIContainer _container;


        public TowerCardUpgradingViewModel TowerUpgrading = new();
        public TowerCardUpgradingViewModel UpgradingNecessary1 = new();
        public TowerCardUpgradingViewModel UpgradingNecessary2 = new();

        public ReactiveProperty<bool> LimitSelectedCard = new(false);
        public PopupBlacksmithViewModel(DIContainer container)
        {
            _container = container;
            _gameState = container.Resolve<IGameStateProvider>().GameState;
            _towerCardPlanService = container.Resolve<TowerCardPlanService>();


            foreach (var inventoryItem in _gameState.Inventory.Items)
            {
                if (inventoryItem is TowerCard towerCard)
                {
                    BaseListCard.Add(towerCard);
                }
            }

            TowerCardMaps.ObserveAdd().Subscribe(e =>
            {
                var towerViewModel = e.Value;
                towerViewModel.IsSelected.Subscribe(v =>
                {
                    SelectedCard(towerViewModel);
                });
            });
            Sorting();

            //TODO Отслеживать процесс слияния карт, обновлять BaseListCard и Sorting();
        }


        private void Sorting()
        {
            TowerCardMaps.Clear();

            Dictionary<string, Dictionary<TypeEpicCard, List<int>>> array = new();
            foreach (var towerCard in BaseListCard)
            {
                var epic = towerCard.EpicLevel.CurrentValue;

                if (array.TryGetValue(towerCard.ConfigId, out var arrayEpics))
                {
                    if (arrayEpics.TryGetValue(epic, out var listCard))
                    {
                        listCard.Add(towerCard.UniqueId);
                    }
                    else
                    {
                        var list = new List<int> { towerCard.UniqueId };
                        arrayEpics.Add(epic, list);
                    }
                }
                else
                {
                    var list = new List<int> { towerCard.UniqueId };
                    var dictEpic = new Dictionary<TypeEpicCard, List<int>>();
                    dictEpic.Add(epic, list);
                    array.Add(towerCard.ConfigId, dictEpic);
                }
            }

            foreach (var configPair in array)
            {
                foreach (var epicPair in configPair.Value.ToArray())
                {
                    if (epicPair.Value.Count < 3) configPair.Value.Remove(epicPair.Key);
                }
            }

            foreach (var configPair in array.ToArray())
            {
                if (configPair.Value.Count == 0) array.Remove(configPair.Key);
            }

            Debug.Log(JsonConvert.SerializeObject(array, Formatting.Indented));

            foreach (var configPair in array)
            {
                foreach (TypeEpicCard value in Enum.GetValues(typeof(TypeEpicCard)))
                {
                    if (configPair.Value.TryGetValue(value, out var uniqueIds))
                    {
                        foreach (var uniqueId in uniqueIds)
                        {
                            var towerCard = _gameState.Inventory.Get<TowerCard>(uniqueId);
                            TowerCardMaps.Add(new TowerCardResourceViewModel(towerCard, _container, LimitSelectedCard));
                        }
                    }
                }
            }
        }

        private bool SelectedCard(TowerCardResourceViewModel towerViewModel)
        {
            //Debug.Log(towerViewModel.TowerEntityId);
            if (towerViewModel.IsSelected.Value)
            {
                //Назначаем карты
                if (TowerUpgrading.IsSetCard.CurrentValue) //Главная назначен
                {
                    //TODO Сделать проверка когда нужна всего 1 карта И НАДО ЛИ?
                    //Проверяем требуемые карты Necessary
                    if (UpgradingNecessary1.IsSetCard.CurrentValue && !UpgradingNecessary2.IsSetCard.CurrentValue)
                    {
                        //Если 1 занята, а 2 свободна
                        UpgradingNecessary2.SetTowerCardViewModel(towerViewModel);
                        LimitSelectedCard.OnNext(true);
                    }
                    else
                    {
                        UpgradingNecessary1.SetTowerCardViewModel(towerViewModel);
                        if (UpgradingNecessary2.IsSetCard.CurrentValue) LimitSelectedCard.OnNext(true);
                    }
                }
                else //Главная не назначена
                {
                    //Назначаем главную и показываем требуемые карты
                    TowerUpgrading.SetTowerCardNecessary(towerViewModel);
                    TowerUpgrading.SetTowerCardViewModel(towerViewModel);
                    UpgradingNecessary1.SetTowerCardNecessary(towerViewModel);
                    UpgradingNecessary2.SetTowerCardNecessary(towerViewModel);
                }
            }
            else
            {
                //Убираем карты
                if (TowerUpgrading.TowerEntityId == towerViewModel.TowerEntityId)
                {
                    TowerUpgrading.ResetNecessary();
                    UpgradingNecessary1.ResetTowerCard();
                     UpgradingNecessary2.ResetTowerCard();
                     
                    UpgradingNecessary1.ResetNecessary();
                    
                    UpgradingNecessary2.ResetNecessary();
                   
                }

                if (UpgradingNecessary1.TowerEntityId == towerViewModel.TowerEntityId) UpgradingNecessary1.ResetViewModel();
                if (UpgradingNecessary2.TowerEntityId == towerViewModel.TowerEntityId) UpgradingNecessary2.ResetViewModel();
                LimitSelectedCard.OnNext(false);
            }

            return true;
        }
        
    }
}