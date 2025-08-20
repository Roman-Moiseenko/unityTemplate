using System;
using Game.State.Maps.Mobs;
using UnityEngine;

namespace Game.GamePlay.View.UI.PanelGateWave
{
    [Serializable]
    public class MobDefenceImage
    {
        public MobDefence mobDefence = MobDefence.Advanced;
        public Sprite image = null;
        
    }
}