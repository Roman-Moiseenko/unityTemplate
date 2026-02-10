using System.Collections.Generic;
using Game.State.Maps.Mobs;
using Game.State.Maps.Towers;

namespace Game.State.Research
{
    public class GameplayBoosters
    {
        public float TowerDamage { get; set; }
        public float TowerCritical { get; set; }
        public float TowerDistance { get; set; }
        public float TowerSpeed { get; set; }
        public float RewardCurrency { get; set; }
        public float SkillDamage { get; set; }

        public float HeroDamage { get; set; }
        public int CastleHealth { get; set; }
        public float CastleDamage{ get; set; }
        public float CastleSpeed { get; set; }
        
        public float CastleRegenerate { get; set; }

        //TODO Доработать
        
        //Бустеры на башню от героя, по типам башни 
        public Dictionary<MobDefence, Dictionary<TowerParameterType, float>> HeroTowerDefenceBust = new();
        //Бустеры на башню от героя
        public Dictionary<TowerParameterType, float> HeroTowerBust = new();
    }
}