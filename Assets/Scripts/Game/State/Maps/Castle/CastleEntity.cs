using Game.State.Entities;
using R3;

namespace Game.State.Maps.Castle
{
    public class CastleEntity : Entity
    {
        public ReactiveProperty<int> CurrenHealth;
        public int FullHealth;
        public int ReduceHealth;
        public float Damage;
        public float DistanceDamage;
        public ReactiveProperty<bool> IsDead; //Для подписок
        
        public CastleEntity(CastleEntityData castleData) : base(castleData)
        {
            FullHealth = castleData.FullHealth;
            ReduceHealth = castleData.ReduceHealth;
            Damage = castleData.Damage;
            DistanceDamage = castleData.DistanceDamage;

            CurrenHealth = new ReactiveProperty<int>(castleData.CurrenHealth);
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