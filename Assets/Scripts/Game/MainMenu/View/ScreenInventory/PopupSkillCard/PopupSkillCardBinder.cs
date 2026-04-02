using System;
using System.Collections.Generic;
using Game.Common;
using Game.GameRoot.ImageManager;
using Game.MainMenu.View.ScreenInventory.Parameters;
using Game.State.Common;
using Game.State.Inventory.SkillCards;
using Game.State.Maps.Skills;
using MVVM.UI;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenInventory.PopupSkillCard
{
    public class PopupSkillCardBinder : PopupBinder<PopupSkillCardViewModel>
    {
        [SerializeField] private TMP_Text textSoftCurrency;

        [SerializeField] private TMP_Text textTitle;
        [SerializeField] private TMP_Text textDeck;
        [SerializeField] private TMP_Text textUpgrade;
        [SerializeField] private TMP_Text textCost;
        
        [SerializeField] private TMP_Text textDescription;

        [SerializeField] private Image epicImage;
        [SerializeField] private Image skillImage;
        [SerializeField] private Image planImage;
        [SerializeField] private TMP_Text textLevel;
        [SerializeField] private TMP_Text textPlanAmount;

        [SerializeField] private Button buttonUpgrade;
        [SerializeField] private Button buttonDeck;

        [SerializeField] private List<Transform> parameters = new(6);
        [SerializeField] private List<ParameterBinder> _parameterBinders;
        
        private IDisposable _disposable;

        protected override void OnBind(PopupSkillCardViewModel viewModel)
        {
            base.OnBind(viewModel);
            var d = Disposable.CreateBuilder();
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            
            parameters.ForEach(item =>
            {
                item.gameObject.SetActive(false);
                _parameterBinders.Add(item.GetComponent<ParameterBinder>());
            });

            viewModel.ViewModel.IsDeck.Subscribe(v =>
            {
                buttonDeck.transform.Find("textDeck").GetComponent<TMP_Text>().text = v ? "УБРАТЬ" : "ВЗЯТЬ";
            }).AddTo(ref d);
            
            var skillCardViewModel = viewModel.ViewModel;
            var skillSettings = viewModel.ViewModel.SkillSettings;
            
            var skillCardData = (SkillCardData)skillCardViewModel.SkillCard.Origin;
            textTitle.text = $"{skillSettings.TitleLid} ({skillCardViewModel.EpicLevel.CurrentValue.GetString()})";
            textDescription.text = skillSettings.DescriptionLid;
            epicImage.sprite = imageManager.GetEpicLevel(skillCardViewModel.EpicLevel.CurrentValue);
            skillImage.sprite = imageManager.GetSkillCard(skillCardViewModel.ConfigId);
            planImage.sprite = imageManager.GetSkillPlan(skillCardViewModel.ConfigId);

            textLevel.text = $"Ур.: {skillCardData.Level}/{skillCardViewModel.SkillCard.MaxLevel()}";
            ViewModel.ViewModel.SkillCard.Level
                .Subscribe(newLevel =>
                {
                    textLevel.text = $"Ур.: {newLevel}/{skillCardViewModel.SkillCard.MaxLevel()}";
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
                textSoftCurrency.text = MyFunc.CurrencyToStr(newValue);
               // buttonUpgrade.gameObject.SetActive(ViewModel.CardIsUpgrade());
            }).AddTo(ref d);
            ViewModel.ViewModel.IsCanUpdate
                .Subscribe(v => buttonUpgrade.gameObject.SetActive(v))
                .AddTo(ref d);
            
            

            var index = 0;
            foreach (var parameter in skillCardViewModel.SkillCard.Parameters)
            {
                _parameterBinders[index].Bind(
                    imageManager.GetSkillParameter(parameter.Key),
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
            buttonDeck.onClick.AddListener(OnSkillCardChangeDeck);
        }

        private void OnDisable()
        {
            buttonUpgrade.onClick.RemoveListener(OnUpgradeLevelCard);
            buttonDeck.onClick.RemoveListener(OnSkillCardChangeDeck);
        }

        private void OnSkillCardChangeDeck()
        {
            ViewModel.SkillCardChangeDeck();
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