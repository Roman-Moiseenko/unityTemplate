using Game.State.Entities;
using UnityEngine;

namespace Game.State.Maps.Roads
{
    public class RoadEntityData
    {
        public int UniqueId { get; set; } //Уникальный ID сущности
        public string ConfigId { get; set; } //Идентификатор для поиска настроек сущности
        public Vector2Int Position { get; set; } //Позиция в координатах x y сущности на карте, конвертируются в x z на плоскости
        
        public Vector2Int PointEnter; 
        public Vector2Int PointExit;
        public int Rotate; //Кол-во поворотов на 90 - от 0 до 3


        
        //public TypeDirection Direction;
    }
}