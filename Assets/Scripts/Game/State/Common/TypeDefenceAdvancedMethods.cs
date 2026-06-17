using System;

namespace Game.State.Common
{
    internal static class TypeDefenceAdvancedMethods
    {
        public static TypeDefence ToDefence(this TypeDefenceAdvanced type)
        {
            return type switch
            {
                TypeDefenceAdvanced.Fast => TypeDefence.Fast,
                TypeDefenceAdvanced.Advanced => TypeDefence.Advanced,
                TypeDefenceAdvanced.Elemental => TypeDefence.Elemental,
                TypeDefenceAdvanced.Support => TypeDefence.Support,
                TypeDefenceAdvanced.All => throw new Exception($"TypeDefenceAdvanced = {type}  Not Find"),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}