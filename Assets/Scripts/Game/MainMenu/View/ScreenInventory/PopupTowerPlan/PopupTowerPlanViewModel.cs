using Game.MainMenu.View.ScreenInventory.TowerPlans;
using MVVM.UI;

namespace Game.MainMenu.View.ScreenInventory.PopupTowerPlan
{
    public class PopupTowerPlanViewModel : WindowViewModel
    {
        public override string Id => "PopupTowerPlan";
        public override string Path => "MainMenu/ScreenInventory/Popups/";

        public readonly TowerPlanViewModel PlanViewModel;

        public PopupTowerPlanViewModel(TowerPlanViewModel planViewModel)
        {
            PlanViewModel = planViewModel;
        }
    }
}