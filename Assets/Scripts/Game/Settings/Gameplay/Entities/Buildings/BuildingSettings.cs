using Game.Settings.Gameplay.Buildings;
using UnityEngine;

namespace Game.Settings.Gameplay.Entities.Buildings
{
    
    [CreateAssetMenu(fileName = "BuildingSettings", 
        menuName = "Game Settings/Buildings/New Building Settings")]
    public class BuildingSettings : EntitySettings<BuildingLevelSettings>
    {
    }
}