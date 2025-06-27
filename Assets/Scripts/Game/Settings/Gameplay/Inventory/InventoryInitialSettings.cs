using System.Collections.Generic;
using UnityEngine;

namespace Game.Settings.Gameplay.Inventory
{
    [CreateAssetMenu(fileName = "InventoryInitialSettings", 
        menuName = "Game Settings/Inventory/New Inventory Settings")]
    public class InventoryInitialSettings : ScriptableObject
    {
        public List<TowerCardSettings> TowerCards;
    }
}