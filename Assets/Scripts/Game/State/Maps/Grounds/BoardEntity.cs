using R3;
using UnityEngine;

namespace Game.State.Maps.Grounds
{
    public class BoardEntity
    {
        public BoardEntityData Origin { get; }
        public int UniqueId => Origin.UniqueId;
        public string ConfigId => Origin.ConfigId;
        
        public readonly ReactiveProperty<Vector2Int> Position;
        public readonly ReactiveProperty<int> Angle;

        public BoardEntity(BoardEntityData boardData)
        {
            Origin = boardData;
            Position = new ReactiveProperty<Vector2Int>(boardData.Position);
            Position.Subscribe(newPosition => boardData.Position = newPosition); 
        }

        public bool EqualsData(BoardEntityData val)
        {
            return
                Origin.TopAngle == val.TopAngle && Origin.TopSide == val.TopSide &&
                Origin.RightAngle == val.RightAngle && Origin.RightSide == val.RightSide &&
                Origin.BottomAngle == val.BottomAngle && Origin.BottomSide == val.BottomSide &&
                Origin.LeftAngle == val.LeftAngle && Origin.LeftSide == val.LeftSide;
        }
    }
}