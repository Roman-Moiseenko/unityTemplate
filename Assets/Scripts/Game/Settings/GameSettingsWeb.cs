using Game.Settings.Gameplay.Enemies;
using Game.Settings.Gameplay.Entities.Tower;
using Game.Settings.Gameplay.Initial;
using Game.Settings.Gameplay.Maps;

namespace Game.Settings
{
    public class GameSettingsWeb
    {
        public MapsSettingsWeb MapsSettings = new();
        public TowersSettingsWeb TowersSettings = new();
        public CastleInitialSettingsWeb CastleInitialSettings = new();
        public InventoryInitialSettingsWeb InventoryInitialSettings = new();
        public MobsSettingsWeb MobsSettings = new();
    }
}