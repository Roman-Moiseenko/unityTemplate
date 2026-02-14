using Game.State.Maps.Mobs;
using R3;

namespace Game.GamePlay.View
{
    /**
     * View-модели, которые могут получать урон (имеют здоровье)
     */
    public interface IHasHeathViewModel
    {
        public int UniqueId { get; }
        public ReadOnlyReactiveProperty<bool> IsDead { get; }
        
        /**
         * Второй параметр используется в Warrior
         */
        public void DamageReceived(float damage, MobDefence defence);

        //TODO Добавить периодический урон (от ожога, укуса, яда)
    }
}