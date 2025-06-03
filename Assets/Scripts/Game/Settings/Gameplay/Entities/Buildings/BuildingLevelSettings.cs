using UnityEngine;

namespace Game.Settings.Gameplay.Entities.Buildings
{
    
    [CreateAssetMenu(fileName = "BuildingLevelSettings", 
        menuName = "Game Settings/Entities/Buildings/New Build Level Settings")]
    public class BuildingLevelSettings : EntityLevelSettings
    {
        [field: SerializeField] public double BaseIncome { get; private set; }
    }
}