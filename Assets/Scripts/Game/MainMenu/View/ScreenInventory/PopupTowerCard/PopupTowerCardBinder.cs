using System;
using System.Collections.Generic;
using System.Globalization;
using Game.Common;
using Game.GameRoot.ImageManager;
using Game.MainMenu.View.ScreenInventory.Parameters;
using Game.State.Inventory;
using Game.State.Inventory.TowerCards;
using Game.State.Maps.Towers;
using MVVM.UI;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenInventory.PopupTowerCard
{
    public class PopupTowerCardBinder : PopupBinder<PopupTowerCardViewModel>
    {
        [SerializeField] private TMP_Text textSoftCurrency;

        [SerializeField] private TMP_Text textTitle;
        [SerializeField] private TMP_Text textDeck;
        [SerializeField] private TMP_Text textUpgrade;
        [SerializeField] private TMP_Text textCost;
        
        [SerializeField] private TMP_Text textDescription;

        [SerializeField] private Image epicImage;
        [SerializeField] private Image towerImage;
        [SerializeField] private Image planImage;
        [SerializeField] private TMP_Text textLevel;
        [SerializeField] private TMP_Text textPlanAmount;

        [SerializeField] private Button buttonUpgrade;
        [SerializeField] private Button buttonDeck;

        [SerializeField] private List<Transform> parameters = new(6);
        [SerializeField] private List<ParameterBinder> _parameterBinders;

        private IDisposable _disposable;
        protected override void OnBind(PopupTowerCardViewModel viewModel)
        {
            base.OnBind(viewModel);
            var d = Disposable.CreateBuilder();
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            
            parameters.ForEach(item =>
            {
                item.gameObject.SetActive(false);
                _parameterBinders.Add(item.GetComponent<ParameterBinder>());
            });

            viewModel.CardViewModel.IsDeck.Subscribe(v =>
            {
                buttonDeck.transform.Find("textDeck").GetComponent<TMP_Text>().text = v ? "УБРАТЬ" : "ВЗЯТЬ";
            }).AddTo(ref d);
            
            var towerCardViewModel = viewModel.CardViewModel;
            var towerSettings = viewModel.CardViewModel.TowerSettings;
            
            var towerCardData = (TowerCardData)towerCardViewModel.TowerCard.Origin;
            textTitle.text = $"{towerSettings.TitleLid} ({towerCardViewModel.EpicLevel.CurrentValue.GetString()})";
            textDescription.text = towerSettings.DescriptionLid;
            epicImage.sprite = imageManager.GetEpicLevel(towerCardViewModel.EpicLevel.CurrentValue);
            towerImage.sprite = imageManager.GetTowerCard(towerCardViewModel.ConfigId, 1);
            planImage.sprite = imageManager.GetTowerPlan(towerCardViewModel.ConfigId);

            textLevel.text = $"Ур.: {towerCardData.Level}/{towerCardViewModel.TowerCard.MaxLevel()}";
            ViewModel.CardViewModel.TowerCard.Level
                .Subscribe(newLevel =>
                {
                    textLevel.text = $"Ур.: {newLevel}/{towerCardViewModel.TowerCard.MaxLevel()}";
                })
                .AddTo(ref d);
            
            ViewModel.CostCurrency.Subscribe(newCost => textCost.text = newCost.ToString()).AddTo(ref d);
            ViewModel.CostPlan.Merge(ViewModel.AmountPlans).Subscribe(_ =>
            {
                textPlanAmount.text = $"{ViewModel.AmountPlans.CurrentValue} / {ViewModel.CostPlan.CurrentValue}";
                
                 //Проверяем стоимость и наличие 
            }).AddTo(ref d);

            ViewModel.SoftCurrency.Subscribe(newValue =>
            {
                textSoftCurrency.text = Func.CurrencyToStr(newValue);
               // buttonUpgrade.gameObject.SetActive(ViewModel.CardIsUpgrade());
            }).AddTo(ref d);
            ViewModel.CardViewModel.IsCanUpdate
                .Subscribe(v => buttonUpgrade.gameObject.SetActive(v))
                .AddTo(ref d);
            
            
            //textCost.text = towerCardViewModel.TowerCard.GetCostCurrencyLevelUpTowerCard().ToString();

            var index = 0;
            foreach (var parameter in towerCardViewModel.TowerCard.Parameters)
            {
                _parameterBinders[index].Bind(
                    imageManager.GetParameter(parameter.Key),
                    parameter.Key.GetString(),
                    parameter.Key.GetMeasure(),
                    parameter.Value.Value
                    );
                index++;
            }
            
            _disposable = d.Build();
        }

        private void OnEnable()
        {
            buttonUpgrade.onClick.AddListener(OnUpgradeLevelCard);
            buttonDeck.onClick.AddListener(OnTowerCardChangeDeck);
        }

        private void OnDisable()
        {
            buttonUpgrade.onClick.RemoveListener(OnUpgradeLevelCard);
            buttonDeck.onClick.RemoveListener(OnTowerCardChangeDeck);
        }

        private void OnTowerCardChangeDeck()
        {
            ViewModel.TowerCardChangeDeck();
            ViewModel.RequestClose();
        }

        private void OnUpgradeLevelCard()
        {
            ViewModel.LevelUpCard();
        }

        protected override void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}