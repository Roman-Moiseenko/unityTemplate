using DI;
using Game.MainMenu.Services;
using Game.MainMenu.View.ScreenInventory.TowerCards;
using Game.State;
using Game.State.Inventory;
using Game.State.Inventory.TowerPlans;
using Game.State.Root;
using MVVM.UI;
using R3;

namespace Game.MainMenu.View.ScreenInventory.PopupTowerCard
{
    public class PopupTowerCardViewModel : WindowViewModel
    {
        public readonly TowerCardViewModel CardViewModel;
        public override string Id => "PopupTowerCard";
        public override string Path => "MainMenu/ScreenInventory/Popups/";

        private InventoryRoot _inventory;
        private TowerCardPlanService _service;
        
        public ReadOnlyReactiveProperty<long> SoftCurrency;
        public ReadOnlyReactiveProperty<long> AmountPlans;
        public ReactiveProperty<long> CostPlan = new();
        public ReactiveProperty<int> CostCurrency = new();
        
        public PopupTowerCardViewModel(TowerCardViewModel viewModel, DIContainer container)
        {
            CardViewModel = viewModel;
            _inventory = container.Resolve<IGameStateProvider>().GameState.Inventory;
            _service = container.Resolve<TowerCardPlanService>();
            SoftCurrency = container.Resolve<IGameStateProvider>().GameState.SoftCurrency;
            
            var plan = _inventory.GetByConfigAndType<TowerPlan>(InventoryType.TowerPlan, viewModel.ConfigId);
            AmountPlans = plan == null ? new ReactiveProperty<long>(0) : plan.Amount;
            
            viewModel.Level.Subscribe(newLevel =>
            {
                CostPlan.OnNext(viewModel.TowerCard.GetCostPlanLevelUpTowerCard());
                CostCurrency.OnNext(viewModel.TowerCard.GetCostCurrencyLevelUpTowerCard());
            });
        }

        public void LevelUpCard()
        {
            _service.LevelUpTowerCard(CardViewModel.IdTowerCard);
        }

        public void TowerCardChangeDeck()
        {
            _service.ChangeDeckTowerCard(CardViewModel.IdTowerCard);
        }

        public bool CardIsUpgrade()
        {
            return (AmountPlans.CurrentValue >= CostPlan.CurrentValue) && 
                   (SoftCurrency.CurrentValue >= CostCurrency.CurrentValue) && 
                   (CardViewModel.TowerCard.MaxLevel() > CardViewModel.TowerCard.Level.CurrentValue);
        }
    }
}