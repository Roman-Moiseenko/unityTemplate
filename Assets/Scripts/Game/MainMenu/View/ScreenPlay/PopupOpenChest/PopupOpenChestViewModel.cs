using System.Collections.Generic;
using DI;
using Game.Common;
using Game.MainMenu.Services;
using Game.MainMenu.View.ScreenPlay.PopupFinishGameplay.PrefabBinders;
using Game.State;
using Game.State.Inventory.Chests;
using Game.State.Root;
using MVVM.UI;
using R3;
using UnityEngine;

namespace Game.MainMenu.View.ScreenPlay.PopupOpenChest
{
    public class PopupOpenChestViewModel : WindowViewModel
    {
        private readonly DIContainer _container;
        private readonly ChestService _chestService;
        public override string Id => "PopupOpenChest";
        public override string Path => "MainMenu/ScreenPlay/Popups/";
        public Chest Chest;
        private readonly GameStateProxy _gameState;
        public bool IsOpening;
        public bool IsClose;
        public int CostOpened;
        public ReactiveProperty<int> CostCurrentOpened = new(0);
        public ReactiveProperty<int> TimeLeftCurrent = new(0);
        
        public List<ResourceRewardViewModel> RewardResources = new();
        private readonly PlayUIManager _uiManager;

        public PopupOpenChestViewModel(Chest chest, DIContainer container)
        {
            Chest = chest;
            _container = container;
            _uiManager = container.Resolve<PlayUIManager>();
            _chestService = container.Resolve<ChestService>();
            _gameState = container.Resolve<IGameStateProvider>().GameState;
            IsOpening = _gameState.ContainerChests.CellOpening.CurrentValue != 0;
            var openingChest = _gameState.ContainerChests.OpeningChest();
            if (openingChest == null)
            {
                IsClose = true;
                //Сундуки не открываются
            }
            else
            {
                IsClose = false;
                if (chest.Cell != openingChest.Cell)
                {
                    IsOpening = false;
                    CostOpened = (int)(chest.TypeChest.FullHourOpening()*60 * AppConstants.RATIO_COST_OPEN_CHEST);
                    //Другой сундук открывается
                }
                else
                {
                    IsOpening = true;
                    TimeLeftCurrent = _chestService.TimeLeft;
                    CostCurrentOpened = _chestService.CostLeft;
                    //Текущий сундук открываются
                }
                
            }
            
            //Получить предварительный объем награды от типа сундука
            var rewards = _chestService.GetListRewards(chest);
            foreach (var rewardItem in rewards)
            {
                var viewModel = new ResourceRewardViewModel
                {
                    InventoryType = rewardItem.RewardType,
                    ConfigId = rewardItem.ConfigId,
                    Amount = rewardItem.Amount,
                };
                RewardResources.Add(viewModel);
            }
        }

        public void RequestOpeningChest()
        {
            //
            _chestService.OpeningChest(Chest);
            RequestClose();
        }

        public void RequestCurrentForcedOpened()
        {
            _chestService.ForcedCurrentChest(Chest);
            RequestClose();
            
        }

        public void RequestCurrentOpenedChest()
        {
            //Открываем сундук, получаем награды
            var rewards =_chestService.OpenCurrentChest(Chest);
            RequestClose();
            //Показываем награды
            _uiManager.OpenPopupRewardChest(Chest.TypeChest, rewards);
        }
        
    }
}