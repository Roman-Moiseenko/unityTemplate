using UnityEngine;

namespace Game.State.Maps.Roads
{
    public class RoadPoint
    {
        public Vector2 Point;
        public Vector2 Direction;

        public RoadPoint(Vector2 point, Vector2 direction)
        {
            Point = point;
            Direction = direction;
        }
    }
}