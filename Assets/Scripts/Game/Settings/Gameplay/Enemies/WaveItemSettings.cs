using System;

namespace Game.Settings.Gameplay.Enemies
{
    [Serializable]
    public class WaveItemSettings
    {
        public int Quantity;
        public MobSettings Mob;
        public int Level = 1;
    }
}