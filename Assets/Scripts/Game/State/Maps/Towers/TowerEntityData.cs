using System.Collections.Generic;
using Game.State.Entities;
using Game.State.Maps.Mobs;
using UnityEngine;

namespace Game.State.Maps.Towers
{
    public class TowerEntityData
    {
        public int UniqueId { get; set; } //Уникальный ID сущности
        public string ConfigId { get; set; } //Идентификатор для поиска настроек сущности
        public int Level { get; set; }
        public Vector2Int Position { get; set; } //Позиция в координатах x y сущности на карте, конвертируются в x z на плоскости
        
        public TowerTypeEnemy TypeEnemy { get; set; }
        //Список усилений, навешанных на башню

        public bool IsOnRoad;
       // public Dictionary<TowerParameterType, TowerParameterData> Parameters = new();
        
        public bool IsMultiShot { get; set; }
        public bool IsSingleTarget { get; set; }
        public float SpeedShot { get; set; }
        public MobDefence Defence { get; set; }
        public Vector2Int Placement { get; set; } //Доп.размещение, для Гарнизона точка появления солдат
        public bool IsPlacement { get; set; }
    }
}