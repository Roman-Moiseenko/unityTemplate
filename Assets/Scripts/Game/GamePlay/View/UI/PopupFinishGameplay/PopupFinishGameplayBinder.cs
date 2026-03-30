using System;
using System.Collections.Generic;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.View.UI.Statistics;
using Game.GameRoot.ImageManager;
using Game.GameRoot.View.ResourceReward;
using MVVM.UI;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PopupFinishGameplay
{
    public class PopupFinishGameplayBinder : PopupBinder<PopupFinishGameplayViewModel>
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Transform winTitle;
        [SerializeField] private Transform loseTitle;
        [SerializeField] private TMP_Text textKillAmount;
        [SerializeField] private TMP_Text textLastWave;

        //[SerializeField] private Image imageMode;
        //  [SerializeField] private TMP_Text textMode;

        [SerializeField] private TMP_Text textTimeAmount;
        [SerializeField] private TMP_Text textMapNumber;

        [SerializeField] private Transform containerResources;
        [SerializeField] private ChestRewardBinder chestBinder;

        [SerializeField] private AllDamageStatistics allDamageStatistics;
        [SerializeField] private ScrollElementsStatistics scrollElementsStatistics;

        private readonly List<ResourceRewardBinder> _createdRewardMap = new();
        private IDisposable _disposable;

        protected override void OnBind(PopupFinishGameplayViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            base.OnBind(viewModel);
            allDamageStatistics.Bind(viewModel.AllDamage);
            scrollElementsStatistics.Bind(viewModel.Elements);
            backgroundImage.material.SetInt("_IsWin", viewModel.EnterParams.FinishedMap ? 1 : 0);


            winTitle.gameObject.SetActive(ViewModel.EnterParams.FinishedMap);
            loseTitle.gameObject.SetActive(!ViewModel.EnterParams.FinishedMap);
            textKillAmount.text = ViewModel.EnterParams.KillsMob.ToString();
            textLastWave.text = ViewModel.EnterParams.LastWave.ToString();

            var minutes = Mathf.FloorToInt(ViewModel.TotalTimeInScene / 60);
            var seconds = Mathf.FloorToInt(ViewModel.TotalTimeInScene % 60);
            textTimeAmount.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            textMapNumber.text = $"Глава {ViewModel.MapId}";
            ViewModel.RewardResources.ForEach(CreateResourceCard);
            UpdateHeightContainerResource();
            chestBinder.Bind(ViewModel.RewardChest);

            _disposable = d.Build();
        }

        private void CreateResourceCard(ResourceRewardViewModel viewModel)
        {
            const string prefabReward = "Prefabs/UI/Common/ResourceReward";
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