using System;
using Game.State.Maps.Towers;
using UnityEngine;

namespace Game.GameRoot.ImageManager
{
    [Serializable]
    public class ImageItemByTowerParameter
    {
        public TowerParameterType TypeParameter;
        public Sprite Sprite;
    }
}