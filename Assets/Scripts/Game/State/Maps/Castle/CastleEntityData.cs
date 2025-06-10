using Game.State.Entities;

namespace Game.State.Maps.Castle
{
    public class CastleEntityData : EntityData
    {
        public int CurrenHealth { get; set; }
        public int FullHealth { get; set; }
        public int ReduceHealth { get; set; }
        public float Damage { get; set; }
        public float DistanceDamage { get; set; }
    }
}