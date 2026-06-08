using System;
using Game.State.Maps.Skills;
using Game.State.Maps.Towers;
using Game.State.Parameter;
using UnityEngine;

namespace Game.GameRoot.ImageManager
{
    [Serializable]
    public class ImageItemByParameter
    {
        public ParameterType Parameter;
        public Sprite Sprite;
    }
}