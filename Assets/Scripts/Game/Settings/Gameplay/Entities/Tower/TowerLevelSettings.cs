using System.Collections.Generic;
using UnityEngine;

namespace Game.Settings.Gameplay.Entities.Tower
{
    
    [CreateAssetMenu(fileName = "TowerLevelSettings", 
        menuName = "Game Settings/Towers/New Tower Level Settings")]
    public class TowerLevelSettings : ScriptableObject
    {
        [field: SerializeField] public int Level { get; private set; }
        [field: SerializeField] public string PrefabSkinPath { get; private set; }
        [field: SerializeField] public List<TowerParameterSettings> Parameters;

    }
}