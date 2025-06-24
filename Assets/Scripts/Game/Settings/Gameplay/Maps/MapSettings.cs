using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Settings.Gameplay.Maps
{
    [CreateAssetMenu(fileName = "MapSettings", 
        menuName = "Game Settings/Maps/New Map Settings")]
    public class MapSettings: ScriptableObject
    {
        public int MapId;
        public MapInitialStateSettings InitialStateSettings;
        public MapInfoStateSettings InfoStateSettings;
    }
}