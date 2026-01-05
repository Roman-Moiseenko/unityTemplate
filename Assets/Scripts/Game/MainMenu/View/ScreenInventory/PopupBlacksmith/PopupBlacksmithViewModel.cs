using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.MainMenu.Commands.TowerCommands;
using Game.MainMenu.Services;
using Game.MainMenu.View.ScreenInventory.PopupBlacksmith.PrefabBinders;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State;
using Game.State.Inventory;
using Game.State.Inventory.TowerCards;
using Game.State.Root;
using MVVM.CMD;
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

        public TowerCardUpgradingViewModel TowerUpgrading = new();
        public TowerCardUpgradingViewModel UpgradingNecessary1 = new();
        public TowerCardUpgradingViewModel UpgradingNecessary2 = new();

        public ReactiveProperty<bool> LimitSelectedCard = new(false);
        public ReactiveProperty<int> MaxLevel = new(1);
        private readonly ICommandProcessor _cmd;

        public PopupBlacksmithViewModel(DIContainer container) : base(container)
        {
            _gameState = container.Resolve<IGameStateProvider>().GameState;
            _towerCardPlanService = container.Resolve<TowerCardPlanService>();
            _cmd = container.Resolve<ICommandProcessor>();
            
            TowerCardMaps.ObserveAdd().Subscribe(e =>
            {
                var towerViewModel = e.Value;
                towerViewModel.IsSelected.Subscribe(v =>
                {
                    SelectedCard(towerViewModel);
                });
            });
            TowerUpgrading.IsSetCard
                .Merge(UpgradingNecessary1.IsSetCard)
                .Merge(UpgradingNecessary2.IsSetCard)
                .Subscribe(_ =>
                {
                    var l0 = TowerUpgrading.Level;
                    var l1 = UpgradingNecessary1.Level;
                    var l2 = UpgradingNecessary2.Level;
                    MaxLevel.Value = new[] { l0, l1, l2 }.Max();
                });
            
            UpdateBaseListCard();
            Sorting();
        }

        private void UpdateBaseListCard()
        {
            BaseListCard.Clear();
            foreach (var inventoryItem in _gameState.Inventory.Items)
            {
                if (inventoryItem is TowerCard towerCard)
                {
                    BaseListCard.Add(towerCard);
                }
            }
        }
        
        private void Sorting()
        {
            TowerCardMaps.Clear();

            Dictionary<string, Dictionary<TypeEpicCard, List<int>>> array = new();
            foreach (var towerCard in BaseListCard)
            {
                if (TowerUpgrading.IsSetCard.CurrentValue && 
                    TowerUpgrading.ConfigId != towerCard.ConfigId) continue;
                
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


            foreach (var configPair in array)
            {
                foreach (TypeEpicCard value in Enum.GetValues(typeof(TypeEpicCard)))
                {
                    if (configPair.Value.TryGetValue(value, out var uniqueIds))
                    {
                        foreach (var uniqueId in uniqueIds)
                        {
                            var towerCard = _gameState.Inventory.Get<TowerCard>(uniqueId);
                            TowerCardMaps.Add(new TowerCardResourceViewModel(towerCard, Container, LimitSelectedCard));
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

        public InfoUpgradedViewModel GetInfoUpdates()
        {
            return _towerCardPlanService.GetInfoUpgradedViewModel(
                TowerUpgrading.ConfigId, TowerUpgrading.EpicLevel,
                MaxLevel.CurrentValue
                );
        }

        public void MergeTowerCard()
        {

            var command = new CommandTowerCardAdd();
            command.ConfigId = TowerUpgrading.ConfigId;
            command.Level = MaxLevel.CurrentValue;
            command.EpicLevel = TowerUpgrading.EpicLevel.Next();
            _cmd.Process(command);

            var commandDelete = new CommandTowerCardSpend();
            commandDelete.UniqueId = TowerUpgrading.TowerEntityId;
            _cmd.Process(commandDelete);
            
            commandDelete.UniqueId = UpgradingNecessary1.TowerEntityId;
            _cmd.Process(commandDelete);
            
            commandDelete.UniqueId = UpgradingNecessary2.TowerEntityId;
            _cmd.Process(commandDelete);
            
            UpgradingNecessary2.ResetNecessary();
            UpgradingNecessary1.ResetNecessary();
            TowerUpgrading.ResetNecessary();
            UpdateBaseListCard();
            Sorting();
        }
    }
}