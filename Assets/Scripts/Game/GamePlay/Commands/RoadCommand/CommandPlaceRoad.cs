using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.RoadCommand
{
    public class CommandPlaceRoad : ICommand
    {
        public readonly Vector2Int PointEnter;
        public readonly Vector2Int PointExit;
        public readonly string RoadTypeId;
        public readonly Vector2Int Position;
        public bool IsMainWay;
        public int Rotate;

        public CommandPlaceRoad(
            string roadTypeId, 
            Vector2Int position, 
            Vector2Int pointEnter, 
            Vector2Int pointExit,
            int rotate,
            bool isMainWay = true)
        {
            PointEnter = pointEnter;
            PointExit = pointExit;
            Rotate = rotate;
            IsMainWay = isMainWay;
            RoadTypeId = roadTypeId;
            Position = position;
        }
    }
}