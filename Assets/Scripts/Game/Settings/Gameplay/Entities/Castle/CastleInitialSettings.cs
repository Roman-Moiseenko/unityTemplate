using UnityEngine;

namespace Game.Settings.Gameplay.Entities.Castle
{
    [CreateAssetMenu(fileName = "CastleInitialSettings", 
        menuName = "Game Settings/Castle/New Castle Settings")]
    public class CastleInitialSettings : EntitySettings
    {
        [field: SerializeField] public int FullHealth;
        [field: SerializeField] public int ReduceHealth;
        [field: SerializeField] public float Damage;
        [field: SerializeField] public float DistanceDamage;
    }
}