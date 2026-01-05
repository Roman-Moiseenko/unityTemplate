using System.Collections.Generic;
using DI;
using Game.GamePlay.Root;
using Game.GameRoot.View.ResourceReward;
using Game.MainMenu.Root;
using Game.State.Inventory;
using Game.State.Inventory.Chests;
using Game.State.Maps.Rewards;
using MVVM.UI;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.UI.PopupFinishGameplay
{
    public class PopupFinishGameplayViewModel : WindowViewModel
    {
        public readonly MainMenuEnterParams EnterParams;

        public TypeChest RewardChest;
        public override string Id => "PopupFinishGameplay";
        public override string Path => "Gameplay/Popups/";
        public List<ResourceRewardViewModel> RewardResources = new();
        private readonly List<RewardEntityData> _rewards = new();
        private readonly GameplayExitParams _exitParams;
        private readonly Subject<GameplayExitParams> _exitSceneRequest;

        public PopupFinishGameplayViewModel(
            GameplayExitParams exitParams, 
            Subject<GameplayExitParams> exitSceneRequest,
            DIContainer container) : base(container)
        {
            
            EnterParams = exitParams.MainMenuEnterParams;
            _exitParams = exitParams;
            _exitSceneRequest = exitSceneRequest;
           // var gameState = container.Resolve<IGameStateProvider>().GameState;

            _rewards.Add(new RewardEntityData
            {
                RewardType = InventoryType.SoftCurrency,
                ConfigId = "Currency",
                Amount = EnterParams.SoftCurrency,
            });

            foreach (var rewardCard in EnterParams.RewardCards)
            {
                var element = _rewards.Find(v =>
                    v.RewardType == rewardCard.RewardType &&
                    v.ConfigId == rewardCard.ConfigId);
                if (element != null)
                {
                    element.Amount += rewardCard.Amount;
                }
                else
                {
                    _rewards.Add(rewardCard);
                }
            }

            foreach (var rewardEntity in EnterParams.RewardOnWave)
            {
                _rewards.Add(rewardEntity);
            }

            foreach (var reward in _rewards)
            {
                var viewModel = new ResourceRewardViewModel(reward);
                RewardResources.Add(viewModel);
            }
            
            RewardChest = EnterParams.TypeChest;
        }
        public override void RequestClose()
        {
            base.RequestClose();
            _exitSceneRequest.OnNext(_exitParams);
        }
        
        private void RewardLoad(List<RewardEntityData> list)
        {
        }
    }
}