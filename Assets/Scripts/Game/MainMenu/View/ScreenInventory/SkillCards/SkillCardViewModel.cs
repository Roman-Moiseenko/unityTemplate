using System;
using DI;
using Game.MainMenu.Services;
using Game.Settings.Gameplay.Entities.Skill;
using Game.State;
using Game.State.Common;
using Game.State.Inventory.Common;
using Game.State.Inventory.SkillCards;
using Game.State.Inventory.SkillPlans;
using R3;

namespace Game.MainMenu.View.ScreenInventory.SkillCards
{
    public class SkillCardViewModel : IDisposable
    {
        public SkillCard SkillCard => _skillCardEntity;
        public readonly SkillSettings SkillSettings;
        public string ConfigId => _skillCardEntity.ConfigId;
        public ReadOnlyReactiveProperty<int> Level => _skillCardEntity.Level;

        public int IdSkillCard => _skillCardEntity.UniqueId;

        public int NumberCardDeck { get; set; }
        public ReactiveProperty<TypeEpic> EpicLevel => _skillCardEntity.EpicLevel;

        public ReactiveProperty<bool> IsDeck = new(false);

        private readonly SkillCard _skillCardEntity;
        private readonly SkillCardPlanService _planService;

        private readonly DIContainer _container;

        //Обновление карты
        public ReadOnlyReactiveProperty<long> SoftCurrency;
        public ReadOnlyReactiveProperty<long> AmountPlans;
        public ReactiveProperty<int> CostPlan = new();
        public ReactiveProperty<int> CostCurrency = new();
        public ReactiveProperty<bool> IsCanUpdate = new();
        private DisposableBag _disposables = new();

        public SkillCardViewModel(
            SkillCard skillCardEntity,
            SkillSettings skillSettings,
            SkillCardPlanService planService,
            DIContainer container
        )
        {
            SkillSettings = skillSettings;
            _skillCardEntity = skillCardEntity;
            _planService = planService;
            _container = container;

            var inventory = container.Resolve<IGameStateProvider>().GameState.Inventory;
            SoftCurrency = container.Resolve<IGameStateProvider>().GameState.SoftCurrency;

            var plan = inventory.GetByConfigAndType<SkillPlan>(InventoryType.SkillPlan, ConfigId);
            AmountPlans = plan == null ? new ReactiveProperty<long>(0) : plan.Amount;
            Level.Subscribe(newLevel =>
            {
                CostPlan.OnNext(SkillCard.GetCostPlanLevelUpSkillCard());
                CostCurrency.OnNext(SkillCard.GetCostCurrencyLevelUpSkillCard());
            }).AddTo(ref _disposables);

            Observable
                .Merge(SkillCard.Level, CostCurrency)
                .Subscribe(_ => IsCanUpdate.OnNext(CardCanUpdate()))
                .AddTo(ref _disposables);
            AmountPlans
                .Subscribe(_ => IsCanUpdate.OnNext(CardCanUpdate()))
                .AddTo(ref _disposables);
            SoftCurrency
                .Subscribe(_ => IsCanUpdate.OnNext(CardCanUpdate()))
                .AddTo(ref _disposables);
        }

        public void RequestOpenPopupSkillCard()
        {
            _container.Resolve<Subject<SkillCardViewModel>>().OnNext(this);
        }

        private bool CardCanUpdate()
        {
            return (AmountPlans.CurrentValue >= CostPlan.CurrentValue) &&
                   (SoftCurrency.CurrentValue >= CostCurrency.CurrentValue) &&
                   (SkillCard.MaxLevel() > SkillCard.Level.CurrentValue);
        }

        public void Dispose()
        {
            IsDeck?.Dispose();
            // НЕ дизпоузим SoftCurrency, AmountPlans — это внешние ссылки на GameState/Inventory.
            // Дизпоуз приведёт к ObjectDisposedException при повторном входе в MainMenu
            // SoftCurrency?.Dispose();
            // AmountPlans?.Dispose();
            CostPlan?.Dispose();
            CostCurrency?.Dispose();
            IsCanUpdate?.Dispose();
            _disposables.Dispose();
        }
    }
}