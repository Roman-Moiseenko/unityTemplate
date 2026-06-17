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
        public float RewardCurrency { get; set; }
        
        //Бустеры по типам Defence 
        public Dictionary<TypeDefence, Dictionary<ParameterType, float>> TowerDefenceBust = new();
        public Dictionary<TypeDefence, Dictionary<ParameterType, float>> SkillDefenceBust = new();
        //Бустеры общие
        public readonly Dictionary<ParameterType, float> TowerBust = new();
        public readonly Dictionary<ParameterType, float> HeroBust = new();
        public readonly Dictionary<ParameterType, float> SkillBust = new();
        //TODO Реализовать и добавить параметр - CastleRegenerate
        public readonly Dictionary<ParameterType, float> CastleBust = new();
    }
}