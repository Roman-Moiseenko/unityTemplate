using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Settings.Gameplay.Enemies
{

    public class WaveSettings
    {
        public List<WaveItemSettings> WaveItems = new();
        public List<WaveItemSettings> WaveSecondItems = new();
    }
}