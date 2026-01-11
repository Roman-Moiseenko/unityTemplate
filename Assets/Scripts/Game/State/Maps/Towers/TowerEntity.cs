using System.Collections.Generic;
using Game.State.Entities;
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
        public EntityType Type => Origin.Type;
        public bool IsOnRoad => Origin.IsOnRoad;
        public readonly ReactiveProperty<Vector2Int> Position;
        
        public readonly TowerTypeEnemy TypeEnemy;
        public readonly bool IsMultiShot;
        public readonly bool IsSingleTarget;
        public readonly float SpeedShot;
        
        public ReactiveProperty<bool> IsShot = new(false);
        public ReactiveProperty<bool> IsBusy = new(false);
        public MobDefence Defence => Origin.Defence;
        
        public Dictionary<TowerParameterType, TowerParameterData> Parameters = new();

        public ReactiveProperty<Vector2> PrepareShot = new();

        public ObservableList<MobEntity> Targets = new(); //ID мобов целей
        
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
            SpeedShot = towerEntityData.SpeedShot;
            IsSingleTarget = towerEntityData.IsSingleTarget;

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
/*
            var start = new Vector3(Position.CurrentValue.x, 0.1f, Position.CurrentValue.y);
            var end = new Vector3(Position.CurrentValue.x + 0.5f, 0.1f, Position.CurrentValue.y);
            Debug.DrawLine(start, end);
*/
            //Башня на дороге, нет дистанции
            return Vector2.Distance(mobPosition, Position.CurrentValue) <= 0.5f;
        }

        public bool SetTarget(MobEntity mobEntity)
        {
            //Debug.Log("Проверяем моба " + mobEntity.UniqueId);
            //Проверка на совпадение типа врага и башни
            if (!IsTargetForAttack(mobEntity.IsFly)) return false;
            //Проверка на дистанцию до моба
            if (MobDistanceShot(mobEntity.Position.CurrentValue))
            {
//                Debug.Log("Дистанция");
                //TODO Проверить есть ли уже в списке
                foreach (var target in Targets)
                {
                    if (target == mobEntity)
                    {
//                        Debug.Log("Попытка добавить цель повторно");
                        return false;
                    }
                }
                
                
                Targets.Add(mobEntity);
                //IsShot.Value = true;
                return true;
            }

            return false;
        }

        public ShotEntityData GetShotParameters(MobEntity mobEntity)
        {
            var damage = 0f;
            //Расчет урона от башни
            if (Parameters.TryGetValue(TowerParameterType.Damage, out var parameter))
                damage = parameter.Value;
            
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
            //Critical damage
            var damageType = DamageType.Normal;
            if (Parameters.TryGetValue(TowerParameterType.Critical, out var criticalParameter))
            {
                var shans = Mathf.FloorToInt(100 / criticalParameter.Value);
                if (Mathf.FloorToInt(Mathf.Abs(Random.insideUnitSphere.x) * 999) % shans == 0)
                {
                    damageType = DamageType.Critical;
                    damage *= 2.0f;
                }
            }
            
            if (Defence.Previous() == mobEntity.Defence) damage *= 0.8f;
            if (Defence.Next() == mobEntity.Defence) damage *= 1.2f;
            
            var shotEntityData = new ShotEntityData
            {
                
                TowerEntityId = UniqueId,
                MobEntityId = mobEntity.UniqueId,
                ConfigId = ConfigId,

                Speed = SpeedShot, 
                Single = IsSingleTarget,
                
                Damage = damage, 
                Debuff = debuff,
                DamageType = damageType,
            };

            return shotEntityData;
        }

        public void RemoveTarget(MobEntity mobEntity)
        {
            Targets.Remove(mobEntity);
          //  Debug.Log("Цель удалили " + UniqueId + " Targets = " + Targets.Count);
        }
    }
}