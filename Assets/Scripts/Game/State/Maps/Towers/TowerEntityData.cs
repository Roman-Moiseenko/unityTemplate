﻿using System.Collections.Generic;
using Game.State.Entities;
using UnityEngine;

namespace Game.State.Maps.Towers
{
    public class TowerEntityData
    {
        public int UniqueId { get; set; } //Уникальный ID сущности
        public string ConfigId { get; set; } //Идентификатор для поиска настроек сущности
        public int Level { get; set; }
        public EntityType Type { get; set; } //Тип сущности
        public Vector2Int Position { get; set; } //Позиция в координатах x y сущности на карте, конвертируются в x z на плоскости
        
        public TowerTypeEnemy TypeEnemy { get; set; }
        //Список усилений, навешанных на башню

        public bool IsOnRoad;
       // public Dictionary<TowerParameterType, TowerParameterData> Parameters = new();
        
        public bool IsMultiShot { get; set; }
        
        
    }
}