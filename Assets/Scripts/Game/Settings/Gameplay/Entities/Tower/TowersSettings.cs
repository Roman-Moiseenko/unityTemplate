using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Settings.Gameplay.Entities.Tower
{
    [CreateAssetMenu(fileName = "TowersSettings", 
        menuName = "Game Settings/Towers/New Towers Settings")]
    public class TowersSettings : ScriptableObject
    {
        public List<TowerSettings> AllTowers;
    }
}