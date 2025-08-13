using Game.State.Entities;
using UnityEngine;

namespace Game.State.Maps.Castle
{
    public class CastleEntityData 
    {
        
        public string ConfigId { get; set; } //Идентификатор для поиска настроек сущности
        public Vector2Int Position { get; set; } = Vector2Int.zero; //Позиция в координатах x y сущности на карте, конвертируются в x z на плоскости
        public Vector2Int Direction { get; set; } = Vector2Int.right;//Направление на дорогу, по умолчанию (1,0)
        public Vector2Int DirectionSecond { get; set; } = Vector2Int.zero;//Направление на 2ю дорогу, если есть, то 1-я = (1,-1) 2-я = (1,1). По умолчанию (0, 0)
        public float CurrenHealth { get; set; }
        public float FullHealth { get; set; }
        public float ReduceHealth { get; set; }
        public float Damage { get; set; }
        public float Speed { get; set; }
        public bool IsReduceHealth { get; set; }
        
    }
}