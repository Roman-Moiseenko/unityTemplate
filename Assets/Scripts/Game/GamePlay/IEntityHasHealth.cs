namespace Game.GamePlay
{
    public interface IEntityHasHealth
    {
        public void DamageReceived(float damage);

        public bool IsDeadEntity();
    }
}