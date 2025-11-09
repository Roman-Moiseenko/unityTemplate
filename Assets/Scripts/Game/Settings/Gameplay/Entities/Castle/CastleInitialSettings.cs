using UnityEngine;

namespace Game.Settings.Gameplay.Entities.Castle
{
    [CreateAssetMenu(fileName = "CastleInitialSettings", 
        menuName = "Game Settings/Castle/New Castle Settings")]
    public class CastleInitialSettings : ScriptableObject
    {
        [field: SerializeField] public string ConfigId { get; private set; }
        [field: SerializeField] public string TitleLid { get; private set; }
        [field: SerializeField] public string DescriptionLid { get; private set; }
        [field: SerializeField] public string PrefabPath { get; private set; }
        
        [field: SerializeField] public int FullHealth;
        [field: SerializeField] public int ReduceHealth;
        [field: SerializeField] public float Damage;
        [field: SerializeField] public float Speed;
        
    }
}