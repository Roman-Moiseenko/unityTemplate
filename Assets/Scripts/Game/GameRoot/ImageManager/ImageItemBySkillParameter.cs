using System;
using Game.State.Maps.Skills;
using Game.State.Maps.Towers;
using UnityEngine;

namespace Game.GameRoot.ImageManager
{
    [Serializable]
    public class ImageItemBySkillParameter
    {
        public SkillParameterType TypeParameter;
        public Sprite Sprite;
    }
}