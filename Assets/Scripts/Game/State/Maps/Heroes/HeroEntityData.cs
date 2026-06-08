using System;
using Game.State.Common;

namespace Game.State.Maps.Heroes
{
    public class HeroEntityData
    {
        public int UniqueId { get; set; } //Уникальный ID сущности
        public string ConfigId { get; set; } //Идентификатор для поиска настроек сущности
        public TypeDefence Defence { get; set; }
        public int Rank { get; set; }
        public int Level  { get; set; }
        public TypeTarget TypeTarget { get; set; }
        
        //Вынести в Parameters
        
        public float Damage { get; set; }
        public float Speed { get; set; }
        public float Range { get; set; } //Дальность, по умолчанию 1-2
        public float Crit { get; set; } //Шанс крита 
        
        
        
    }
}