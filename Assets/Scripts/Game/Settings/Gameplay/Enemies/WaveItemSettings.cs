using System;
using UnityEngine.Serialization;

namespace Game.Settings.Gameplay.Enemies
{
    public class WaveItemSettings
    {
        public int Quantity; //Кол-во мобов в пакете
       // public MobSettings Mob;
        public int Level = 1; //Уровень мобов в пакете
        public string MobConfigId; //Конфиг мобов в пакете
    }
}