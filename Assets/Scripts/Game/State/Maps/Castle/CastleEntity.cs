using Game.GamePlay;
using Game.State.Entities;
using Game.State.Maps.Mobs;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.State.Maps.Castle
{
    public class CastleEntity : IEntityHasHealth
    {
        public CastleEntityData Origin;
        public string ConfigId => Origin.ConfigId; //Идентификатор для поиска настроек сущности
        public Vector2Int Position => Origin.Position;  //Позиция в координатах x y сущности на карте, конвертируются в x z на плоскости
        public Vector2Int Direction => Origin.Direction; //Направление на дорогу, по умолчанию (1,0)
        public Vector2Int DirectionSecond => Origin.DirectionSecond; //Направление на 2ю дорогу, если есть, то 1-я = (1,-1) 2-я = (1,1). По умолчанию (0, 0)
        
        public ReactiveProperty<float> CurrenHealth;
        public float FullHealth => Origin.FullHealth;
        public float ReduceHealth => Origin.ReduceHealth;
        public float Damage => Origin.Damage;
        //public float DistanceDamage => Origin.DistanceDamage;
        public float Speed => Origin.Speed;
        public ReactiveProperty<bool> IsDead; //Для подписок

        public ReactiveProperty<bool> IsBusy = new(false);

        public ReactiveProperty<bool> IsReduceHealth;
        public ReactiveProperty<int> CountResurrection;
        public ObservableList<MobEntity> Target = new();
        
        public CastleEntity(CastleEntityData castleData)
        {
            Origin = castleData;
            IsDead = new ReactiveProperty<bool>(false);
            
            CurrenHealth = new ReactiveProperty<float>(castleData.CurrenHealth);
            CurrenHealth.Subscribe(newValue => castleData.CurrenHealth = newValue);

            IsReduceHealth = new ReactiveProperty<bool>(castleData.IsReduceHealth);
            IsReduceHealth.Subscribe(newValue => castleData.IsReduceHealth = newValue);
            
            CountResurrection = new ReactiveProperty<int>(castleData.CountResurrection);
            CountResurrection.Subscribe(newValue => castleData.CountResurrection = newValue);
        }

        public void CastleEntityInitialization(CastleEntityData castleData)
        {
            Origin = castleData;
            CurrenHealth.Value = castleData.FullHealth;
        }

        /**
         * Восстановление
         */
        public void Repair()
        {
            CurrenHealth.Value += ReduceHealth;
            if (CurrenHealth.Value > Origin.FullHealth) CurrenHealth.Value = Origin.FullHealth;
        }

        /**
         * Нанесен урон
         */
        public void DamageReceived(float damage)
        {
            if (CurrenHealth.CurrentValue <= damage)
            {
                CurrenHealth.Value = 0;
                IsDead.Value = true;
            }
            else
            {
                CurrenHealth.Value -= damage;
            }
        }

        public bool Resurrection()
        {
            if (CountResurrection.CurrentValue == 2) return false;
            CurrenHealth.Value = FullHealth;
            IsDead.Value = false;
            CountResurrection.Value++;
            return true;
        }

        public bool SetTarget(MobEntity mobEntity)
        {
            if (!MobDistanceShotCastle(mobEntity.Position.CurrentValue)) return false;
            Target.Add(mobEntity);
            return true;
        }

        public void RemoveTarget(MobEntity mobEntity)
        {
            Target.Remove(mobEntity);
        }
        
        private bool MobDistanceShotCastle(Vector2 position)
        {
            if (position.x > 3) return false;
            return position.y is <= 1.5f and >= -1.5f;
            //TODO Сделать проверку на точку пути 
        }
    }
}