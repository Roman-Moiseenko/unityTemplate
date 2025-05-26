using UnityEngine;

namespace Game.Settings.Gameplay.Buildings
{
    
    [CreateAssetMenu(fileName = "BuildingLevelSettings", 
        menuName = "Game Settings/Buildings/New Build Level Settings")]
    public class BuildingLevelSettings : ScriptableObject
    {
        public int Level;
        public double BaseIncome;
    }
}