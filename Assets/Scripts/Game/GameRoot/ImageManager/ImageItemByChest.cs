using System;
using Game.State.Inventory.Chests;
using UnityEngine;

namespace Game.GameRoot.ImageManager
{
    [Serializable]
    public class ImageItemByChest
    {
        public TypeChest TypeChest;
        public Sprite Sprite;
    }
}