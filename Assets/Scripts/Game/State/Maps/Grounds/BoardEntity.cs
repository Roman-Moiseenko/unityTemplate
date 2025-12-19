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
                Origin.TopInAngle == val.TopInAngle && Origin.TopSide == val.TopSide &&
                Origin.RightInAngle == val.RightInAngle && Origin.RightSide == val.RightSide &&
                Origin.BottomInAngle == val.BottomInAngle && Origin.BottomSide == val.BottomSide &&
                Origin.LeftInAngle == val.LeftInAngle && Origin.LeftSide == val.LeftSide &&
                Origin.TopOutAngle == val.TopOutAngle && Origin.BottomOutAngle == val.BottomOutAngle &&
                Origin.RightOutAngle == val.RightOutAngle && Origin.LeftOutAngle == val.LeftOutAngle;
        }
    }
}