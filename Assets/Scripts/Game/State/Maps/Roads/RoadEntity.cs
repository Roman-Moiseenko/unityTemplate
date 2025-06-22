using Game.State.Entities;
using R3;
using UnityEngine;

namespace Game.State.Maps.Roads
{
    public class RoadEntity
    {
        public RoadEntityData Origin { get; }
        public int UniqueId => Origin.UniqueId;
        public string ConfigId => Origin.ConfigId;
        public readonly ReactiveProperty<int> Rotate;

        public readonly ReactiveProperty<Vector2Int> Position;
        
        
        public readonly ReactiveProperty<Vector2Int> PointEnter;
        public readonly ReactiveProperty<Vector2Int> PointExit;
        
        public RoadEntity(RoadEntityData roadData)
        {
            Origin = roadData;

            Rotate = new ReactiveProperty<int>(roadData.Rotate);
            Rotate.Subscribe(newRotate => roadData.Rotate = newRotate);
            
            Position = new ReactiveProperty<Vector2Int>(roadData.Position);
            Position.Subscribe(newPosition => roadData.Position = newPosition);
            
            PointEnter = new ReactiveProperty<Vector2Int>(roadData.PointEnter);
            PointEnter.Subscribe(newValue => roadData.PointEnter = newValue);
            
            PointExit = new ReactiveProperty<Vector2Int>(roadData.PointExit);
            PointExit.Subscribe(newValue => roadData.PointExit = newValue);
        }
        
        
        
    }
}