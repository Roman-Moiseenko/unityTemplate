using System.Numerics;
using R3;

namespace Game.State.Maps.Shots
{
    public class ShotEntity
    {
        public ShotEntityData Origin;
        public string ConfigId => Origin.ConfigId;
        public int UniqueId => Origin.UniqueId;
        public Vector3 StartPosition => Origin.StartPosition;
        public int Speed => Origin.Speed;
        
        public ReactiveProperty<Vector3> Position;
        public ReactiveProperty<Vector3> FinishPosition;
        
        public ShotEntity(ShotEntityData shotEntityData)
        {
            Origin = shotEntityData;
            Position = new ReactiveProperty<Vector3>(shotEntityData.Position);
            Position.Subscribe(newValue => shotEntityData.Position = newValue);
            
            FinishPosition = new ReactiveProperty<Vector3>(shotEntityData.FinishPosition);
            FinishPosition.Subscribe(newValue => shotEntityData.FinishPosition = newValue);
            
        }
    }
}