using System;
using System.Collections.Generic;
using Game.Common;
using Game.GameRoot.ImageManager;
using Game.MainMenu.View.ScreenPlay.PopupFinishGameplay.PrefabBinders;
using MVVM.UI;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenPlay.PopupRewardChest
{
    public class PopupRewardChestBinder : PopupBinder<PopupRewardChestViewModel>
    {
        [SerializeField] private Transform containerResources;
        [SerializeField] private Image imageChest;
        
        private readonly List<ResourceRewardBinder> _createdRewardMap = new();
        private IDisposable _disposable;
        protected override void OnBind(PopupRewardChestViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            base.OnBind(viewModel);
            ViewModel.RewardResources.ForEach(CreateResourceCard);
            imageChest.sprite = imageManager.GetChest(ViewModel.TypeChest);
            
            _disposable = d.Build();
        }
        
        private void CreateResourceCard(ResourceRewardViewModel viewModel)
        {
            const string prefabReward = "Prefabs/UI/MainMenu/ScreenPlay/Popups/ResourceReward";
            var rewardPrefab = Resources.Load<ResourceRewardBinder>(prefabReward);
            var createdReward = Instantiate(rewardPrefab, containerResources);
            createdReward.Bind(viewModel);
            _createdRewardMap.Add(createdReward);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var rewardBinder in _createdRewardMap)
            {
                Destroy(rewardBinder.gameObject);
            }

            _disposable.Dispose();
        }
    }
}