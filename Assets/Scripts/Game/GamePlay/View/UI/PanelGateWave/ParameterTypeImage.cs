using System;
using Game.State.Maps.Towers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.GamePlay.View.UI.PanelGateWave
{
    [Serializable]
    public class ParameterTypeImage
    {
        public TowerParameterType typeParameter = TowerParameterType.Health;
        public Sprite image = null;
    }
}