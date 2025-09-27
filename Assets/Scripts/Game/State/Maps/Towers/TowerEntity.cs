using System.Collections.Generic;
using Game.State.Entities;
using Game.State.Maps.Mobs;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.State.Maps.Towers
{
    public class TowerEntity
    {
        public TowerEntityData Origin { get; }
        public int UniqueId => Origin.UniqueId;
        public string ConfigId => Origin.ConfigId;

        public readonly ReactiveProperty<int> Level;
        public EntityType Type => Origin.Type;
        public bool IsOnRoad => Origin.IsOnRoad;
        public readonly ReactiveProperty<Vector2Int> Position;
        
        public readonly TowerTypeEnemy TypeEnemy;
        public readonly bool IsMultiShot;
        
        public ReactiveProperty<bool> IsShot = new(false);
        public ReactiveProperty<bool> IsBusy = new(false);
        public MobDefence Defence => Origin.Defence;
        
        public Dictionary<TowerParameterType, TowerParameterData> Parameters = new();

        public ReactiveProperty<Vector2> PrepareShot = new();
        
        public TowerEntity(TowerEntityData towerEntityData)
        {
            Origin = towerEntityData;
            Position = new ReactiveProperty<Vector2Int>(towerEntityData.Position);
            Position.Subscribe(newPosition => towerEntityData.Position = newPosition); //При изменении позиции Position.Value меняем в данных
            
            Level = new ReactiveProperty<int>(towerEntityData.Level);
            Level.Subscribe(newLevel =>
            {
                towerEntityData.Level = newLevel;
            }); //При изменении позиции Position.Value меняем в данных
            
            TypeEnemy = towerEntityData.TypeEnemy;
            IsMultiShot = towerEntityData.IsMultiShot;

            /*          Parameters = new ObservableDictionary<TowerParameterType, TowerParameter>();
                      Parameters.ObserveAdd().Subscribe(e =>
                      {
                          var type = e.Value.Key;
                          var parameter = e.Value.Value.Origin;
                          towerEntityData.Parameters.Add(type, parameter);
                      });
          */
        }

        public bool PositionNear(Vector2Int position)
        {
            var x = Position.CurrentValue.x;
            var y = Position.CurrentValue.y;
            if (position.x == x && position.y == y - 1) return true;
            if (position.x == x && position.y == y + 1) return true;
            if (position.x == x - 1 && position.y == y) return true;
            if (position.x == x + 1 && position.y == y) return true;
            return false;
        }

        /**
         * Проверяем, может ли башня атаковать моба
         */
        public bool IsTargetForAttack(bool mobEntityIsFly)
        {
            if (TypeEnemy == TowerTypeEnemy.Universal) return true;

            switch (mobEntityIsFly)
            {
                case true when TypeEnemy == TowerTypeEnemy.Air:
                case false when TypeEnemy == TowerTypeEnemy.Ground:
                    return true;
                default:
                    return false;
            }
        }
    }
}