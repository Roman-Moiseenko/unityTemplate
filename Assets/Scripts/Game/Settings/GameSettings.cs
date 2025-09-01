using Game.Settings.Gameplay.Buildings;
using Game.Settings.Gameplay.Enemies;
using Game.Settings.Gameplay.Entities.Buildings;

using Game.Settings.Gameplay.Entities.Castle;
using Game.Settings.Gameplay.Entities.Tower;
using Game.Settings.Gameplay.Inventory;
using Game.Settings.Gameplay.Maps;
using UnityEngine;

namespace Game.Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings/New Game Settings")]
    public class GameSettings : ScriptableObject
    {
       // public BuildingsSettings BuildingsSettings;
        public MapsSettings MapsSettings;
        public TowersSettings TowersSettings;
        public CastleInitialSettings CastleInitialSettings;
        public InventoryInitialSettings InventoryInitialSettings;
        public MobsSettings MobsSettings;
    }
}