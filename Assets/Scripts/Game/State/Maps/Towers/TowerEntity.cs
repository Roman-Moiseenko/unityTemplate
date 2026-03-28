using System.Collections.Generic;
using Game.State.Common;
using Game.State.Gameplay.Statistics;
using Game.State.Maps.Mobs;
using Game.State.Maps.Shots;
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
        public bool IsOnRoad => Origin.IsOnRoad;
        public readonly ReactiveProperty<Vector2Int> Position;
        
        public TypeTarget TypeTarget => Origin.TypeTarget;
        public bool IsMultiShot => Origin.IsMultiShot;
        public bool IsSingleTarget => Origin.IsSingleTarget;
        public bool IsPlacement => Origin.IsPlacement;
        public float SpeedShot => Origin.SpeedShot;
        public readonly ReactiveProperty<Vector2Int> Placement; 
        
        public TypeDefence Defence => Origin.Defence;
        
        public Dictionary<TowerParameterType, TowerParameterData> Parameters = new();
        
        public TowerEntity(TowerEntityData towerEntityData)
        {
            Origin = towerEntityData;
            Position = new ReactiveProperty<Vector2Int>(towerEntityData.Position);
            Position.Subscribe(newPosition => towerEntityData.Position = newPosition); //При изменении позиции Position.Value меняем в данных

            Placement = new ReactiveProperty<Vector2Int>(towerEntityData.Placement);
            Placement.Subscribe(v => towerEntityData.Placement = v);
            
            Level = new ReactiveProperty<int>(towerEntityData.Level);
            Level.Subscribe(newLevel =>
            {
                towerEntityData.Level = newLevel;
            }); //При изменении позиции Position.Value меняем в данных
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
            if (TypeTarget == TypeTarget.Universal) return true;

            switch (mobEntityIsFly)
            {
                case true when TypeTarget == TypeTarget.Air:
                case false when TypeTarget == TypeTarget.Ground:
                    return true;
                default:
                    return false;
            }
        }

        /**
         * Дистаниця до моба достаточна для выстрела
         */
        public bool MobDistanceShot(Vector2 mobPosition)
        {
            var d = Vector2.Distance(mobPosition, Position.CurrentValue) - 0.5f; //Отнимаем радиус башни
            //У башни мин. и макс. дистанция
            if (Parameters.TryGetValue(TowerParameterType.MinDistance, out var distanceMin) &&
                Parameters.TryGetValue(TowerParameterType.MaxDistance, out var distanceMax))
                return distanceMin.Value <= d && d <= distanceMax.Value;
            // у башни стандартная дистанция 
            if (Parameters.TryGetValue(TowerParameterType.Distance, out var distance))
            {
                return d <= distance.Value;
            }
            //Башня на дороге, нет дистанции
            return Vector2.Distance(mobPosition, Position.CurrentValue) <= 0.5f;
        }
        
        public ShotData ShotCalculation(TypeDefence typeDefence, float damageBooster, float criticalBooster)
        {
            var damage = 0f;
            if (Parameters.TryGetValue(TowerParameterType.Damage, out var parameter)) damage = parameter.Value;
            
            MobDebuff debuff = null;
            if (Parameters.TryGetValue(TowerParameterType.DamageArea, out parameter))
                damage = parameter.Value;

            //Добавляем дебафф к выстрелу
            if (Parameters.TryGetValue(TowerParameterType.SlowingDown, out var slowParameter))
            {
                var speedTower = 1f;
                if (Parameters.TryGetValue(TowerParameterType.Speed, out var speedParameter))
                    speedTower = speedParameter.Value; //Скорость выстрела == время действия дебафа
                
                debuff = new MobDebuff
                {
                    Value = slowParameter.Value,
                    Type = MobDebuffType.Speed,
                    Time = speedTower,
                };
            }

            damage += damage * damageBooster / 100; //Добавляем бустер урона
            
            var damageType = IsSingleTarget ? DamageType.Normal : DamageType.MassDamage; 
            if (Parameters.TryGetValue(TowerParameterType.Critical, out var criticalParameter))
            {
                var shans = Mathf.FloorToInt(100 / (criticalParameter.Value + criticalBooster)); //Добавляем бустер крита
                if (Mathf.FloorToInt(Mathf.Abs(Random.insideUnitSphere.x) * 999) % shans == 0)
                {
                    damageType = DamageType.Critical;
                    damage *= 2.0f;
                }
            }
            if (Defence.Previous() == typeDefence) damage *= 0.8f;
            if (Defence.Next() == typeDefence) damage *= 1.2f;
            var shotData = new ShotData
            {
                //MobEntityId = mobEntity.UniqueId,
                ConfigId = ConfigId,
                Single = IsSingleTarget,
                Damage = damage, 
                Debuff = debuff,
                DamageType = damageType,
                TypeEntity = TypeEntityStatisticDamage.Tower,
            };
            
            return shotData;
        }

    }
}