using Game.State.Common;
using UnityEngine;

namespace Game.State.Maps.Towers
{
    public class TowerEntityData
    {
        public int UniqueId { get; set; } //Уникальный ID сущности
        public string ConfigId { get; set; } //Идентификатор для поиска настроек сущности
        public int GameplayLevel { get; set; }
        public Vector2Int Position { get; set; } //Позиция в координатах x y сущности на карте, конвертируются в x z на плоскости
        
        public TypeTarget TypeTarget { get; set; }
        //Список усилений, навешанных на башню

        public bool IsOnRoad;
       // public Dictionary<TowerParameterType, TowerParameterData> Parameters = new();
        
        public bool IsMultiShot { get; set; } //Одновременно несколько выстрелов, по все в радиусе целям
        public bool IsSingleTarget { get; set; } //Урон нескольким мобам вокруг цели
        public float SpeedShot { get; set; }
        public TypeDefence Defence { get; set; }
        public Vector2Int Placement { get; set; } //Доп.размещение, для Гарнизона точка появления солдат
        public bool IsPlacement { get; set; }
    }
}