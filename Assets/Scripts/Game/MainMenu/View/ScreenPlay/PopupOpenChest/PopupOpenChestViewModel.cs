using System.Collections.Generic;
using DI;
using Game.Common;
using Game.GameRoot.View.ResourceReward;
using Game.MainMenu.Services;
using Game.State;
using Game.State.Inventory.Chests;
using Game.State.Root;
using MVVM.UI;
using R3;

namespace Game.MainMenu.View.ScreenPlay.PopupOpenChest
{
    public class PopupOpenChestViewModel : WindowViewModel
    {
        private readonly ChestService _chestService;
        public override string Id => "PopupOpenChest";
        public override string Path => "MainMenu/ScreenPlay/Popups/";
        public InfoRewardViewModel InfoRewardViewModel;

        public Chest Chest;
        private readonly GameStateProxy _gameState;
        public bool IsOpening;
        public bool IsClose;
        public int CostOpened;
        public ReactiveProperty<int> CostCurrentOpened = new(0);
        public ReactiveProperty<int> TimeLeftCurrent = new(0);

        public List<ResourceRewardViewModel> RewardResources = new();
        
        private readonly PlayUIManager _uiManager;

        public PopupOpenChestViewModel(Chest chest, DIContainer container) : base(container)
        {
            Chest = chest;
            _uiManager = container.Resolve<PlayUIManager>();
            _chestService = container.Resolve<ChestService>();
            _gameState = container.Resolve<IGameStateProvider>().GameState;
            IsOpening = _gameState.ContainerChests.CellOpening.CurrentValue != 0;
            InfoRewardViewModel = new InfoRewardViewModel();
            var openingChest = _gameState.ContainerChests.OpeningChest();
            if (openingChest == null)
            {
                IsClose = true;  //Сундуки не открываются
            }
            else
            {
                IsClose = false;
                if (chest.Cell != openingChest.Cell)
                {
                    IsOpening = false; //Другой сундук открывается
                    CostOpened = (int)(chest.TypeChest.FullHourOpening() * 60 * AppConstants.RATIO_COST_OPEN_CHEST);
                }
                else
                {
                    IsOpening = true; //Текущий сундук открываются
                    TimeLeftCurrent = _chestService.TimeLeft;
                    CostCurrentOpened = _chestService.CostLeft;
                }
            }

            //Получить предварительный объем награды от типа сундука
            var rewards = _chestService.GetListRewards(chest);
            //  Debug.Log(JsonConvert.SerializeObject(rewards, Formatting.Indented));
            foreach (var rewardItem in rewards)
            {
                var viewModel = new ResourceRewardViewModel(rewardItem);
                RewardResources.Add(viewModel);
            }

            var chanceRewards = _chestService.GetListChanceRewards(chest);
            var odd = false;
            foreach (var rewardItem in chanceRewards)
            {
                odd = !odd;
                var viewModel = new ResourceRewardViewModel(rewardItem);
                InfoRewardViewModel.CreateItem(rewardItem, odd);
            }
        }

        public void RequestOpeningChest()
        {
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
            var rewards = _chestService.OpenCurrentChest(Chest);
            RequestClose();
            //Показываем награды
            _uiManager.OpenPopupRewardChest(Chest.TypeChest, rewards);
        }
    }
}