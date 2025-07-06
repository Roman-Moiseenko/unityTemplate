using System.Collections.Generic;
using UnityEngine;

namespace Game.Settings.Gameplay.Enemies
{
    
    [CreateAssetMenu(fileName = "MobsSettings", 
        menuName = "Game Settings/Enemies/New Mobs Settings")]
    public class MobsSettings : ScriptableObject
    {
        public List<MobSettings> AllMobs;

    }
}