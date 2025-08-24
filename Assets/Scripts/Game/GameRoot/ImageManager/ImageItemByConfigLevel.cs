using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameRoot.ImageManager
{
    [Serializable]
    public class ImageItemByConfigLevel
    {
        public string ConfigId;
        public List<ImageItemByLevel> ByLevels;
    }
}