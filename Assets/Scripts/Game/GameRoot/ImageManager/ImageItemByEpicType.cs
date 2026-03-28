using System;
using Game.State.Common;
using Game.State.Inventory;
using UnityEngine;

namespace Game.GameRoot.ImageManager
{
    [Serializable]
    public class ImageItemByEpicType
    {
        public TypeEpic TypeEpic;
        public Sprite Sprite;
    }
}