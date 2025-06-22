using System;
using Game.State.Maps.Roads;
using UnityEngine;

namespace Game.Settings.Gameplay.Entities.Road
{
    [Serializable]
    public class RoadInitialSettings
    {
      //  public Vector2Int PointEnter;
       // public Vector2Int PointExit;
        public Vector2Int Position;
        public string ConfigId;
        public int Rotate;
    }
}