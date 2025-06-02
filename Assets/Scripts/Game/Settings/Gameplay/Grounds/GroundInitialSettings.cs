using System;
using UnityEngine;

namespace Game.Settings.Gameplay.Grounds
{
    [Serializable]
    public class GroundInitialSettings
    {
        public Vector2Int Position;
        public bool Enabled => true;
    }
}