using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.RoadCommand
{
    public class CommandPlaceRoad : ICommand
    {
        public readonly string RoadTypeId;
        public readonly Vector2Int Position;
        public bool IsMainWay;
        public int Rotate;
        public bool IsTurn;

        public CommandPlaceRoad(
            string roadTypeId, 
            Vector2Int position, 
            bool isTurn,
            int rotate,
            bool isMainWay = true)
        {
            IsTurn = isTurn;
            Rotate = rotate;
            IsMainWay = isMainWay;
            RoadTypeId = roadTypeId;
            Position = position;
        }
    }
}