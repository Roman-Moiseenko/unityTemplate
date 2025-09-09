using System;

namespace Game.Settings.Gameplay
{
    [Serializable]
    public class InfinitySetting
    {
        public float ratioCurveWave = 2;
        public float ratioCurveMobs = 2;
        public float rateLevelMob = 10f;
        public int rateBoss = 10;
        public float ratioPower = 1f;
    }
}