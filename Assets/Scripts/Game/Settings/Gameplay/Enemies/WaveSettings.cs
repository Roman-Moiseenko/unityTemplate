using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Settings.Gameplay.Enemies
{
    [Serializable]
    public class WaveSettings
    {

        public List<WaveItemSettings> WaveItems = new();
    }
}