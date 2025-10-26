using Game.Settings.Gameplay.Initial;

namespace Game.Settings.Gameplay.Maps
{
    public class MapSettingsWeb
    {
        public int MapId;
        public MapInitialStateSettingsWeb InitialStateSettings = new();
        public MapInfoStateSettingsWeb InfoStateSettings = new();
    }
}