using Game.State.Inventory;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Settings.Gameplay.Inventory
{
    [CreateAssetMenu(fileName = "InventoryInitialSettings", 
        menuName = "Game Settings/Inventory/New Tower Card")]
    public class TowerCardSettings : ScriptableObject
    {
        [field: SerializeField] public string ConfigId;
        [field: SerializeField] public TypeEpicCard epicCardLevel;
        [field: SerializeField] public int Level;
    }
}