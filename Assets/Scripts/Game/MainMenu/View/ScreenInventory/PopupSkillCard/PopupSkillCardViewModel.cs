using DI;
using Game.MainMenu.Services;
using Game.MainMenu.View.ScreenInventory.SkillCards;
using Game.State;
using Game.State.Inventory.Common;
using Game.State.Inventory.SkillPlans;
using MVVM.UI;
using Newtonsoft.Json;
using R3;
using UnityEngine;

namespace Game.MainMenu.View.ScreenInventory.PopupSkillCard
{
    public class PopupSkillCardViewModel : WindowViewModel
    {
        public override string Id => "PopupSkillCard";
        public override string Path => "MainMenu/ScreenInventory/Popups/";
        
        private InventoryRoot _inventory;
        private SkillCardPlanService _service;
        
        public ReadOnlyReactiveProperty<long> SoftCurrency;
        public ReadOnlyReactiveProperty<long> AmountPlans;
        public ReactiveProperty<long> CostPlan = new();
        public ReactiveProperty<int> CostCurrency = new();
        public SkillCardViewModel ViewModel;
        public PopupSkillCardViewModel(SkillCardViewModel viewModel, DIContainer container) : base(container)
        {
            ViewModel = viewModel;
            _inventory = container.Resolve<IGameStateProvider>().GameState.Inventory;
            _service = container.Resolve<SkillCardPlanService>();
            SoftCurrency = container.Resolve<IGameStateProvider>().GameState.SoftCurrency;
            var plan = _inventory.GetByConfigAndType<SkillPlan>(InventoryType.SkillPlan, viewModel.ConfigId);
            AmountPlans = plan == null ? new ReactiveProperty<long>(0) : plan.Amount;
            
            viewModel.Level.Subscribe(newLevel =>
            {
                CostPlan.OnNext(viewModel.SkillCard.GetCostPlanLevelUpSkillCard());
                CostCurrency.OnNext(viewModel.SkillCard.GetCostCurrencyLevelUpSkillCard());
            });
        }
        public void LevelUpCard()
        {
            _service.LevelUpSkillCard(ViewModel.IdSkillCard);
        }

        public void SkillCardChangeDeck()
        {
            _service.ChangeDeckSkillCard(ViewModel.IdSkillCard);
        }

        public bool CardIsUpgrade()
        {
            return (AmountPlans.CurrentValue >= CostPlan.CurrentValue) && 
                   (SoftCurrency.CurrentValue >= CostCurrency.CurrentValue) && 
                   (ViewModel.SkillCard.MaxLevel() > ViewModel.SkillCard.Level.CurrentValue);
        }

    }
}