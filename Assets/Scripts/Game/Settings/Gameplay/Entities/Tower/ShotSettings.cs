using System;

namespace Game.Settings.Gameplay.Entities.Tower
{
    [Serializable]

    public class ShotSettings
    {
        public float Speed = 1f;
        public bool LineTrajectory = true;
        public bool Single = true;
        public bool NotPrefab = false;
    }
}