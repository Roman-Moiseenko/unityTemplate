using System;
using Game.State.Common;
using Game.State.Maps.Mobs;
using Game.State.Maps.Towers;
using UnityEngine;

namespace Game.GameRoot.ImageManager
{
    [Serializable]
    public class ImageItemByDefence
    {
        public TypeDefence Defence;
        public Sprite Sprite;
    }
}