using DI;
using Game.Settings.Gameplay.Entities.Skill;
using Game.State.Inventory.SkillPlans;
using R3;

namespace Game.MainMenu.View.ScreenInventory.SkillPlans
{
    public class SkillPlanViewModel
    {
        
        public SkillPlan SkillPlan => _skillPlanEntity;
        public int IdSkillPlan => _skillPlanEntity.UniqueId;
        public string ConfigId => _skillPlanEntity.ConfigId;
        public ReadOnlyReactiveProperty<long> Amount => _skillPlanEntity.Amount;
        public readonly SkillSettings SkillSettings;

        private readonly DIContainer _container;
        private readonly SkillPlan _skillPlanEntity;
        public SkillPlanViewModel(
            SkillPlan skillPlanEntity,
            SkillSettings skillSettings,
            DIContainer container
            )
        {
            _skillPlanEntity = skillPlanEntity;
            SkillSettings = skillSettings;
            _container = container;
        }
        
        public void RequestOpenPopupSkillPlan()
        {
            _container.Resolve<Subject<SkillPlanViewModel>>().OnNext(this);
        }
    }
}