using UnityEngine;

namespace Game.State.Maps.Roads
{
    public class RoadPoint
    {
        public Vector2 Point;
        public Vector2Int Direction;

        public RoadPoint(Vector2 point, Vector2Int direction)
        {
            Point = point;
            Direction = direction;
        }
    }
}