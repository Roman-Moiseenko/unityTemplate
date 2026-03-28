using Game.State.Common;

namespace Game.State.Maps.Skills
{
    public class SkillEntityData
    {
        public int UniqueId { get; set; } //Уникальный ID сущности
        public string ConfigId { get; set; } //Идентификатор для поиска настроек сущности
        public int Level { get; set; }
        public TypeTarget TypeTarget { get; set; }
        public bool OnRoad { get; set; }
        
        public TypeDefence Defence { get; set; }

    }
}