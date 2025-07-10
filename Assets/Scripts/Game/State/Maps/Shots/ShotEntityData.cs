using System.Numerics;

namespace Game.State.Maps.Shots
{
    public class ShotEntityData
    {
        public string ConfigId;
        public int UniqueId;
        public int TowerEntityId;
        public Vector3 StartPosition;
        public Vector3 FinishPosition;
        public Vector3 Position;
        public int Speed;
    }
}