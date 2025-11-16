using DI;
using Game.MainMenu.Services;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State;
using Game.State.Inventory;
using Game.State.Inventory.TowerCards;
using Game.State.Inventory.TowerPlans;
using R3;
using UnityEngine;

namespace Game.MainMenu.View.ScreenInventory.TowerCards
{
    public class TowerCardViewModel
    {
        public TowerCard TowerCard => _towerCardEntity;
        public readonly TowerSettings TowerSettings;
        public string ConfigId => _towerCardEntity.ConfigId;
        public ReadOnlyReactiveProperty<TypeEpicCard> EpicLevel => _towerCardEntity.EpicLevel;
        public ReadOnlyReactiveProperty<int> Level => _towerCardEntity.Level;
        public int IdTowerCard => _towerCardEntity.UniqueId;
        public int NumberCardDeck { get; set; } //Номер места в колоде (для присваивания парента)

        public ReactiveProperty<bool> IsDeck = new(false);
        //public ReactiveProperty<bool> IsDeck = new(false);
        
        private readonly TowerCard _towerCardEntity;
        private readonly TowerCardPlanService _planService;
        private readonly DIContainer _container;
        
        //Обновление карты
        public ReadOnlyReactiveProperty<long> SoftCurrency;
        public ReadOnlyReactiveProperty<long> AmountPlans;
        public ReactiveProperty<int> CostPlan = new();
        public ReactiveProperty<int> CostCurrency = new();
        public ReactiveProperty<bool> IsCanUpdate = new();
        
        public TowerCardViewModel(
            TowerCard towerCardEntity, 
            TowerSettings towerSettings, 
            TowerCardPlanService planService,
            DIContainer container
            )
        {
            _towerCardEntity = towerCardEntity;
            TowerSettings = towerSettings;
            _planService = planService;
            _container = container;
            var inventory = container.Resolve<IGameStateProvider>().GameState.Inventory;
            SoftCurrency = container.Resolve<IGameStateProvider>().GameState.SoftCurrency;
            
            var plan = inventory.GetByConfigAndType<TowerPlan>(InventoryType.TowerPlan, ConfigId);
            AmountPlans = plan == null ? new ReactiveProperty<long>(0) : plan.Amount;
            Level.Subscribe(newLevel =>
            {
                CostPlan.OnNext(TowerCard.GetCostPlanLevelUpTowerCard());
                CostCurrency.OnNext(TowerCard.GetCostCurrencyLevelUpTowerCard());
            });

            Observable.Merge(
                TowerCard.Level, CostCurrency
                ).Subscribe(_ => IsCanUpdate.OnNext(CardCanUpdate()));
            AmountPlans.Subscribe(_ => IsCanUpdate.OnNext(CardCanUpdate()));
            SoftCurrency.Subscribe(_ => IsCanUpdate.OnNext(CardCanUpdate()));
            
        }
        
        
        public void RequestOpenPopupTowerCard()
        {
            _container.Resolve<Subject<TowerCardViewModel>>().OnNext(this);
        }

        private bool CardCanUpdate()
        {
            return (AmountPlans.CurrentValue >= CostPlan.CurrentValue) && 
                   (SoftCurrency.CurrentValue >= CostCurrency.CurrentValue) && 
                   (TowerCard.MaxLevel() > TowerCard.Level.CurrentValue);
        }
    }
}