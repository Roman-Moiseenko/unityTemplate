using System;
using System.Collections.Generic;
using System.Globalization;
using Game.Common;
using Game.GamePlay.View.UI.PanelGateWave;
using Game.GameRoot.ImageManager;
using Game.MainMenu.View.ScreenInventory.Parameters;
using Game.State.Inventory;
using Game.State.Inventory.TowerCards;
using Game.State.Maps.Towers;
using MVVM.UI;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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
                buttonUpgrade.gameObject.SetActive(CheckUpgrade());
            }).AddTo(ref d);

            ViewModel.SoftCurrency.Subscribe(newValue =>
            {

                textSoftCurrency.text = CurrencyToStr(newValue);
                //Отображаем общее кол-во денег
                //Проверяем стоимость и наличие
                buttonUpgrade.gameObject.SetActive(CheckUpgrade());
            }).AddTo(ref d);
            //textCost.text = towerCardViewModel.TowerCard.GetCostCurrencyLevelUpTowerCard().ToString();

            var index = 0;
            foreach (var parameter in towerCardViewModel.TowerCard.Parameters)
            {
                //parameter.Value.Value.Subscribe(newValue =>
          //      {
                    //Debug.Log("newvalue" + newValue);
                    var value = (int)(parameter.Value.Value.CurrentValue * 100) / 100f;
                    //parameters[index].Find("RightBlock/ValueBlock/Container/textValueParameter").GetComponent<TMP_Text>().text = value.ToString(CultureInfo.CurrentCulture) + " " + parameter.Key.GetMeasure();  
                    
              //  }
                //).AddTo(ref d);
                _parameterBinders[index].Bind(
                    imageManager.GetParameter(parameter.Key),
                    parameter.Key.GetString(),
                    value.ToString(CultureInfo.CurrentCulture),
                    parameter.Key.GetMeasure(),
                    parameter.Value.Value
                    );
                index++;
            }
            
            _disposable = d.Build();
        }

        private string CurrencyToStr(int value)
        {
            var t = 0f;
            if (value >= 1000000)
            {
                t = (int)(value / 10000) / 100f;
                return $"{t} M";
            }

            if (value >= 1000)
            {
                t = (int)(value / 10) / 100f;
                return $"{t} K";
            }

            return t.ToString();
        }

        private bool CheckUpgrade()
        {
            return (ViewModel.AmountPlans.CurrentValue >= ViewModel.CostPlan.CurrentValue) && 
                   (ViewModel.SoftCurrency.CurrentValue >= ViewModel.CostCurrency.CurrentValue);
        }

        private void OnEnable()
        {
            buttonUpgrade.onClick.AddListener(OnUpgradeLevelCard);
            //buttonDeck.onClick.AddListener();
        }

        private void OnDisable()
        {
            buttonUpgrade.onClick.AddListener(OnUpgradeLevelCard);
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