using UnityEngine;

namespace Game.State.Maps.Grounds
{
    public class BoardEntityData
    {
        public int UniqueId { get; set; } //Уникальный ID сущности
        public string ConfigId { get; set; } //Идентификатор для поиска настроек сущности
        public Vector2Int Position { get; set; } //Позиция в координатах x y сущности на карте, конвертируются в x z на плоскости

        public bool LeftSide { get; set; }
        public bool RightSide { get; set; }
        public bool TopSide { get; set; }
        public bool BottomSide { get; set; }
        
        public bool LeftAngle { get; set; } // -1, 0, 1
        public bool RightAngle { get; set; } // -1, 0, 1
        public bool TopAngle { get; set; } // -1, 0, 1
        public bool BottomAngle { get; set; } // -1, 0, 1
        
        //public int Side = 0;
    }
}