using System.Collections.Generic;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Maps.Towers;
using UnityEngine;

namespace Game.Settings.Gameplay.Entities.Busts
{
    
    [CreateAssetMenu(fileName = "TowerBustSettings", 
        menuName = "Game Settings/Busts/New Tower Bust Settings")]
    public class TowerBustSettings : ScriptableObject
    {
        [field: SerializeField] public int Level;
        [field: SerializeField] public string ConfigId;
        [field: SerializeField] public string Name;
        [field: SerializeField] public TowerTypeBust MainBust;
        [field: SerializeField] public TowerTypeBust SecondBust;
        [field: SerializeField] public double MainAmount;
        [field: SerializeField] public double SecondAmount;

        
    }
}