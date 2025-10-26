using System.Collections.Generic;
using Game.Settings.Gameplay.Inventory;

namespace Game.Settings.Gameplay.Initial
{
    public class InventoryInitialSettingsWeb
    {
        public List<TowerCardSettingsWeb> TowerCards = new();
        public List<TowerPlanSettingsWeb> TowerPlans = new();
    }
}