using System;
using System.Collections.Generic;

namespace Game.Settings.Gameplay.Entities
{
    [Serializable]
    public class LevelSettings
    {
        public int Level;
        public List<ParameterSettings> Parameters = new();

    }
}