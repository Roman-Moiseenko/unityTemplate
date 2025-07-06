using Game.State.Maps.Mobs;
using UnityEngine;

namespace Game.Settings.Gameplay.Enemies
{
    [CreateAssetMenu(fileName = "MobSettings", 
        menuName = "Game Settings/Enemies/New Mob Settings")]
    public class MobSettings : ScriptableObject
    {
        [field: SerializeField] public string ConfigId { get; private set; }
        [field: SerializeField] public string TitleLid { get; private set; }
        [field: SerializeField] public string DescriptionLid { get; private set; }
        [field: SerializeField] public MobType Type { get; private set; }
        
        [field: SerializeField] public string PrefabPath { get; private set; }

        [field: SerializeField] public int Speed { get; private set; }
        [field: SerializeField] public bool IsFly { get; private set; }
        [field: SerializeField] public float Health { get; private set; }
        [field: SerializeField] public float Armor { get; private set; }
        [field: SerializeField] public float Attack { get; private set; }
        
        [field: SerializeField] public int RewardCurrency { get; private set; }

    }
}