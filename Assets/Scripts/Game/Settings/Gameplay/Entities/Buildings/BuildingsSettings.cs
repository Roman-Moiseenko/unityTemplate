using System.Collections.Generic;
using Game.Settings.Gameplay.Buildings;
using UnityEngine;

namespace Game.Settings.Gameplay.Entities.Buildings
{
    [CreateAssetMenu(fileName = "BuildingsSettings", 
        menuName = "Game Settings/Buildings/New Buildings Settings")]
    public class BuildingsSettings : ScriptableObject
    {
        public List<BuildingSettings> AllBuildings;
    }
}