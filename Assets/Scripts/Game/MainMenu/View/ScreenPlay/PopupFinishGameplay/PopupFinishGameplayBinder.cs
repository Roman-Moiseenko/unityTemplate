using System;
using System.Collections.Generic;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GameRoot.ImageManager;
using Game.MainMenu.View.ScreenPlay.PopupFinishGameplay.PrefabBinders;
using MVVM.UI;
using ObservableCollections;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenPlay.PopupFinishGameplay
{
    public class PopupFinishGameplayBinder : PopupBinder<PopupFinishGameplayViewModel>
    {
        [SerializeField] private Transform winTitle;
        [SerializeField] private Transform loseTitle;
        [SerializeField] private TMP_Text textKillAmount;
        [SerializeField] private TMP_Text textLastWave;

        [SerializeField] private Image imageMode;
        [SerializeField] private TMP_Text textMode;

        [SerializeField] private Transform containerResources;
        [SerializeField] private ChestRewardBinder chestBinder;
        private readonly List<ResourceRewardBinder> _createdRewardMap = new();
        private IDisposable _disposable;

        protected override void OnBind(PopupFinishGameplayViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            base.OnBind(viewModel);
            winTitle.gameObject.SetActive(ViewModel.EnterParams.CompletedLevel);
            loseTitle.gameObject.SetActive(!ViewModel.EnterParams.CompletedLevel);
            textKillAmount.text = ViewModel.EnterParams.KillsMob.ToString();
            textLastWave.text = ViewModel.EnterParams.LastWave.ToString();

            if (ViewModel.EnterParams.TypeGameplay == TypeGameplay.Infinity)
            {
                imageMode.sprite = imageManager.GetOther("TypeInfinity");
                //TODO Заменить на imageManager.GetGameplay(ViewModel.EnterParams.TypeGameplay, ConfigId)
            }
            
            textMode.text = ViewModel.EnterParams.TypeGameplay.GetString();
            ViewModel.RewardResources.ForEach(CreateResourceCard);
            UpdateHeightContainerResource();
            chestBinder.Bind(ViewModel.RewardChest);

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
        private void UpdateHeightContainerResource()
        {
            var container = containerResources.GetComponent<RectTransform>();
            const int blockHeight = 170;
            const int blockSpacing = 10;
            var rows = Math.Ceiling(container.childCount / 5f);
            
            container.sizeDelta = container.childCount == 0 
                ? new Vector2(container.sizeDelta.x, 0) 
                : new Vector2(container.sizeDelta.x, (float)(rows * blockHeight + (rows - 1) * blockSpacing));
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