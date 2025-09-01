using System;

namespace Game.Settings.Gameplay.Enemies
{
    [Serializable]
    public class MobParameters
    {
        public int Level;
        public float Health;
        public float Armor;
        public float Attack;
        public int RewardCurrency;
    }
}