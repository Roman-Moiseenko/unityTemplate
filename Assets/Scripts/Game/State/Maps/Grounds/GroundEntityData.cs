using Game.State.Entities;
using UnityEngine;

namespace Game.State.Maps.Grounds
{
    public class GroundEntityData
    {
        public int UniqueId { get; set; } //Уникальный ID сущности
        public string ConfigId { get; set; } //Идентификатор для поиска настроек сущности
        public Vector2Int Position { get; set; } //Позиция в координатах x y сущности на карте, конвертируются в x z на плоскости
     //   public GroundType GroundType { get; set; }
        public bool Enabled { get; set; }
    }
}