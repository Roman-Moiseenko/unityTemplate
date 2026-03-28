using System;
using Game.State.Common;
using Game.State.Inventory;
using UnityEngine;

namespace Game.GameRoot.ImageManager
{
    [Serializable]
    public class ImageEpicData
    {
        public TypeEpic Epic;
        public Sprite Card;
        public Sprite Header;
        public Sprite Output;
        public Sprite Background;
    }
}