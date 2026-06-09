using System;
using Game.State.Common;
using UnityEngine;

namespace Game.State.Maps.Heroes
{
    public class HeroEntityData
    {
        public int UniqueId { get; set; } //Уникальный ID сущности
        public string ConfigId { get; set; } //Идентификатор для поиска настроек сущности
        
        public int Rank { get; set; } //Для информации
        public int Level  { get; set; } //Для информации
        public TypeEpic EpicLevel; //Неизменный, ни на что не влияет 
        public int GameplayLevel  { get; set; } //
        public TypeTarget TypeTarget { get; set; }
        public TypeDefence Defence { get; set; }
        public Vector2Int Placement { get; set; } 
        
        public bool IsSingleTarget { get; set; }
        //Вынести в Parameters
        
    }
}