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
        
        public bool LeftInAngle { get; set; } // -1, 0, 1
        public bool RightInAngle { get; set; } // -1, 0, 1
        public bool TopInAngle { get; set; } // -1, 0, 1
        public bool BottomInAngle { get; set; } // -1, 0, 1
        
        
        public bool LeftOutAngle { get; set; } // -1, 0, 1
        public bool RightOutAngle { get; set; } // -1, 0, 1
        public bool TopOutAngle { get; set; } // -1, 0, 1
        public bool BottomOutAngle { get; set; } // -1, 0, 1
        //public int Side = 0;
    }
}