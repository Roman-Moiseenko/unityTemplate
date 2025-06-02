using System;
using UnityEngine;

namespace Game.State.Entities
{
   // [Serializable]
    public class EntityData
    {
        public int UniqueId { get; set; } //Уникальный ID сущности
        public string ConfigId { get; set; } //Идентификатор для поиска настроек сущности
        public EntityType Type { get; set; } //Тип сущности
        public Vector2Int Position { get; set; } //Позиция в координатах x y сущности на карте, конвертируются в x z на плоскости
        
        
    }
}