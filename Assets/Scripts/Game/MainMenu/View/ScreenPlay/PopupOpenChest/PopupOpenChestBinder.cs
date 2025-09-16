using System;
using System.Collections.Generic;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GameRoot.ImageManager;
using Game.MainMenu.View.ScreenPlay.PopupFinishGameplay.PrefabBinders;
using Game.State.Inventory.Chests;
using MVVM.UI;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenPlay.PopupOpenChest
{
    public class PopupOpenChestBinder : PopupBinder<PopupOpenChestViewModel>
    {
        [SerializeField] private Button buttonOpening;
        
        [SerializeField] private Image imageChest;
        [SerializeField] private TMP_Text textLevel;
        [SerializeField] private TMP_Text textChest;
        [SerializeField] private TMP_Text textTypeGame;
        [SerializeField] private Transform containerResources;
        
        [SerializeField] private Transform isClosed;
        [SerializeField] private TMP_Text textTimeOpening;
        
        [SerializeField] private Transform isOpening;
        [SerializeField] private TMP_Text textCostOpen;
        [SerializeField] private Button buttonOpened;
        
        [SerializeField] private Transform isCurrentOpening;
        [SerializeField] private TMP_Text textTimeLeftCurrent;
        [SerializeField] private TMP_Text textCostCurrentOpen;
        [SerializeField] private Button buttonCurrentOpened;
        [SerializeField] private Button buttonCurrentSpeed;

        private readonly List<ResourceRewardBinder> _createdRewardMap = new();
        private IDisposable _disposable;
        
        protected override void OnBind(PopupOpenChestViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            base.OnBind(viewModel);
            
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            imageChest.sprite = imageManager.GetChest(ViewModel.Chest.TypeChest);
            textLevel.text = $"Уровень {ViewModel.Chest.Level}";
            
            TypeChest? typeChest= ViewModel.Chest.TypeChest;
            textChest.text = typeChest.GetString();
            textTypeGame.text = ViewModel.Chest.Gameplay.GetString();
            
            //Действия над сундуком, от статуса открытия:
            //Сундук закрыт, открывания нет
            isClosed.gameObject.SetActive(ViewModel.IsClose);
            textTimeOpening.text = $"{ViewModel.Chest.TypeChest.FullHourOpening().ToString()} ч";
            
            //Открывается другой сундук
            isOpening.gameObject.SetActive(!ViewModel.IsClose && !ViewModel.IsOpening);
            textCostOpen.text = $"{ViewModel.CostOpened}";
            
            //Открывается текущий сундук
            isCurrentOpening.gameObject.SetActive(!ViewModel.IsClose && ViewModel.IsOpening);
            ViewModel.TimeLeftCurrent.Subscribe(t =>
            {
                textTimeLeftCurrent.text = t / 60 > 0 ? $"{t / 60}ч {t % 60}мин" : $"{t % 60}мин";
            }).AddTo(ref d);
            ViewModel.CostCurrentOpened.Subscribe(c =>
            {
                textCostCurrentOpen.text = $"{c}";
            }).AddTo(ref d);
            
            //Возможные награды
            ViewModel.RewardResources.ForEach(CreateResourceCard);
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

        private void OnEnable()
        {
            buttonOpening.onClick.AddListener(OnOpeningChest);
            buttonOpened.onClick.AddListener(OnCurrentOpenedChest);
            buttonCurrentOpened.onClick.AddListener(OnCurrentOpenedChest);
            buttonCurrentSpeed.onClick.AddListener(OnCurrentForcedOpened);
        }

        private void OnCurrentForcedOpened()
        {
            ViewModel.RequestCurrentForcedOpened();
        }

        private void OnCurrentOpenedChest()
        {
            ViewModel.RequestCurrentOpenedChest();
        }
        
        private void OnDisable()
        {
            buttonOpening.onClick.RemoveListener(OnOpeningChest);
            buttonOpened.onClick.RemoveListener(OnCurrentOpenedChest);
            buttonCurrentOpened.onClick.RemoveListener(OnCurrentOpenedChest);
            buttonCurrentSpeed.onClick.RemoveListener(OnCurrentForcedOpened);
        }

        private void OnOpeningChest()
        {
            ViewModel.RequestOpeningChest();
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