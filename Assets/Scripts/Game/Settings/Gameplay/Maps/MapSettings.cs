using Game.Settings.Gameplay.Initial;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Settings.Gameplay.Maps
{
    public class MapSettings
    {
        public int MapId;
        public MapInitialStateSettings InitialStateSettings = new();
        public MapInfoStateSettings InfoStateSettings = new();
        public MapRewardSetting MapRewardSetting = new();
    }
}