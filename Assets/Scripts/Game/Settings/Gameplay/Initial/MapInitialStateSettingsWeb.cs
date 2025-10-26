using System.Collections.Generic;
using Game.Settings.Gameplay.Enemies;

namespace Game.Settings.Gameplay.Initial
{
    public class MapInitialStateSettingsWeb
    {
        public List<TowerInitialSettingsWeb> Towers = new();
        public List<WaveSettings> Waves = new();
        
        public string groundDefault;
        public bool smallMap = false;
        public bool hasWaySecond = false;
        public bool hasWayDisabled = false;
        public bool obstacle = false; //Препятствия
        public int collapse = 0; //Степень провалов на начальной карте
        public string roadDefault = "Road";
    }
}