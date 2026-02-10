using System;
using Game.State.Maps.Mobs;
using UnityEngine;

namespace Game.GameRoot.ImageManager
{
    [Serializable]
    public class ImageDefenceData
    {
        public MobDefence Defence;
        public Sprite Icon;
        public Sprite Background;
    }
}