using System.Collections.Generic;
using Game.Settings.Gameplay.Inventory;
using UnityEngine;

namespace Game.Settings.Gameplay.Initial
{
    public class InventoryInitialSettings
    {
        public List<TowerCardSettings> TowerCards = new();
        public List<TowerPlanSettings> TowerPlans = new();
        public List<SkillCardSettings> SkillCards = new();
        public List<SkillPlanSettings> SkillPlans = new();
    }
}