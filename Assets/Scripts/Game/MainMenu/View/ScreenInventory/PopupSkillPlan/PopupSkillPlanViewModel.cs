using DI;
using Game.MainMenu.View.ScreenInventory.SkillPlans;
using MVVM.UI;

namespace Game.MainMenu.View.ScreenInventory.PopupSkillPlan
{
    public class PopupSkillPlanViewModel : WindowViewModel
    {
        public override string Id => "PopupSkillPlan";
        public override string Path => "MainMenu/ScreenInventory/Popups/";
        public readonly SkillPlanViewModel ViewModel;

        public PopupSkillPlanViewModel(SkillPlanViewModel viewModel, DIContainer container) : base(container)
        {
            ViewModel = viewModel;
        }
    }
}