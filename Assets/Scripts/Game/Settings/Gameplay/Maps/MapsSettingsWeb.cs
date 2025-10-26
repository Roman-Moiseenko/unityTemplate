using System.Collections.Generic;

namespace Game.Settings.Gameplay.Maps
{
    public class MapsSettingsWeb
    {
        public List<MapSettingsWeb> Maps = new();
        public List<string> GroundConfigIds = new();
        public List<string> RoadConfigIds = new();

        public InfinitySettingWeb InfinitySetting = new();
    }
}