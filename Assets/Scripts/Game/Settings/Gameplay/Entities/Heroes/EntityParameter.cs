using System;
using Game.State.Common;
using Game.State.Parameters;

namespace Game.Settings.Gameplay.Entities.Heroes
{
    [Serializable]
    public class EntityParameter
    {
        public string Name;
        public string Description;
        public TypeEntity Entity;
        public TypeDefenceAdvanced Defence;
        public ParameterType Parameter;
        public int Index;
        public float Value;
    }
}