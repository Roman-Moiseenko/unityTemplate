using System.Collections.Generic;
using Game.State.Common;
using Game.State.Maps.Mobs;
using Game.State.Maps.Skills;
using Game.State.Maps.Towers;
using Game.State.Parameters;

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
        public float CastleDamage { get; set; }
        public float CastleSpeed { get; set; }
        
        public float CastleRegenerate { get; set; }

        //TODO Доработать
        
        //Бустеры по типам Defence 
        public Dictionary<TypeDefence, Dictionary<ParameterType, float>> TowerDefenceBust = new();
        public Dictionary<TypeDefence, Dictionary<ParameterType, float>> SkillDefenceBust = new();
        //Бустеры общие
        public readonly Dictionary<ParameterType, float> TowerBust = new();
        public readonly Dictionary<ParameterType, float> HeroBust = new();
        public readonly Dictionary<ParameterType, float> SkillBust = new();
    }
}