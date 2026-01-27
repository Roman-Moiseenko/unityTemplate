using System.Collections.Generic;
using Game.State.Maps.Mobs;

namespace Game.Settings.Gameplay.Enemies
{
    public class MobSettings
    {
        public string ConfigId;
        public string TitleLid;
        public string DescriptionLid;
        public string PrefabPath;
        public float SpeedMove;
        public float SpeedAttack;
        public bool IsFly;
        public int AvailableWave;
        public int RewardCurrency;
        public MobDefence Defence;
        
        //Все динамические параметры, зависящие от уровня
        public Dictionary<MobParameter, List<float>> Parameters = new();

        //Кол-во мобов на 1 единицу
        public int Count = 1;
    }
}