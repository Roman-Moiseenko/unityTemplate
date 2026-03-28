using System;
using Game.State.Common;
using Game.State.Maps.Mobs;
using UnityEngine;

namespace Game.GameRoot.ImageManager
{
    [Serializable]
    public class ImageDefenceData
    {
        public TypeDefence Defence;
        public Sprite Icon;
        public Sprite Background;
    }
}