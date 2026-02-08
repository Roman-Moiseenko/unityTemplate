using System;
using Game.State.Inventory;
using UnityEngine;

namespace Game.GameRoot.ImageManager
{
    [Serializable]
    public class ImageEpicData
    {
        public TypeEpicCard Epic;
        public Sprite Card;
        public Sprite Header;
        public Sprite Output;
        public Sprite Background;
    }
}