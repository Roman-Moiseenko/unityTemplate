using Game.Settings.Gameplay.Buildings;
using Game.Settings.Gameplay.Entities.Buildings;
using Game.Settings.Gameplay.Entities.Busts;
using Game.Settings.Gameplay.Entities.Tower;
using Game.Settings.Gameplay.Maps;
using UnityEngine;

namespace Game.Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings/New Game Settings")]
    public class GameSettings : ScriptableObject
    {
        public BuildingsSettings BuildingsSettings;
        public MapsSettings MapsSettings;
        public TowerBustsSettings TowerBustsSettings;
        public TowersSettings TowersSettings;
    }
}