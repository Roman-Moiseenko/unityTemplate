using Game.State.Inventory;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Settings.Gameplay.Inventory
{
    [CreateAssetMenu(fileName = "InventoryInitialSettings", 
        menuName = "Game Settings/Inventory/New Tower Plan")]
    public class TowerPlanSettings : ScriptableObject
    {
        [field: SerializeField] public string ConfigId;
        [field: SerializeField] public int Amount;
    }
}