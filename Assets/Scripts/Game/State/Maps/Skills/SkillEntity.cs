using System.Collections.Generic;
using Game.State.Common;
using R3;

namespace Game.State.Maps.Skills
{
    public class SkillEntity
    {
        public SkillEntityData Origin { get; }
        public int UniqueId => Origin.UniqueId;
        public string ConfigId => Origin.ConfigId;
        public readonly ReactiveProperty<int> Level;
        public TypeTarget TypeTarget => Origin.TypeTarget;
        public bool OnRoad => Origin.OnRoad;
        public TypeDefence Defence => Origin.Defence;
        
        public Dictionary<SkillParameterType, SkillParameterData> Parameters = new();
        
        public SkillEntity(SkillEntityData entityData)
        {
            Origin = entityData;
            Level = new ReactiveProperty<int>(entityData.Level);
            Level.Subscribe(x => Origin.Level = x);
        }
    }
}