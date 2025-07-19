using Game.State.Entities;
using R3;
using UnityEngine;

namespace Game.State.Maps.Castle
{
    public class CastleEntity
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

        public ReactiveProperty<bool> IsShot = new(false);
        
        public CastleEntity(CastleEntityData castleData)
        {
            Origin = castleData;
            

            CurrenHealth = new ReactiveProperty<float>(castleData.CurrenHealth);
            CurrenHealth.Subscribe(newValue =>
            {
                castleData.CurrenHealth = newValue;
            });
        }

        /**
         * Идет восстановление
         */
        public void Repair(int speed = 1)
        {
            CurrenHealth.Value += speed * ReduceHealth;
            if (CurrenHealth.Value > 100) CurrenHealth.Value = 100;
            
        }

        /**
         * Нанесен урон
         */
        public void DamageReceived(int damage)
        {
            CurrenHealth.Value -= damage;
            if (CurrenHealth.Value == 0) IsDead.Value = true;
        } 
    }
}