using System;
using Game.State.Maps.Towers;
using UnityEngine;

namespace Game.Settings.Gameplay.Entities.Tower
{
    [Serializable]
    public class ParameterDataSetting
    {
        public TowerParameterType ParameterType;
        public float Value;
    }
}