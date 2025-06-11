using System.Collections.Generic;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Maps.Towers;
using UnityEngine;

namespace Game.Settings.Gameplay.Entities.Busts
{
    
    [CreateAssetMenu(fileName = "TowerBustsSettings", 
        menuName = "Game Settings/Busts/New Tower Busts Settings")]
    public class TowerBustsSettings : ScriptableObject
    {
        [field: SerializeField] public List<TowerBustSettings> Busts;
    }
}