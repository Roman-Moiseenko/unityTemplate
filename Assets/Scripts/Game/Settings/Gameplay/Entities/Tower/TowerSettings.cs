using System.Collections.Generic;
using Game.Settings.Gameplay.Buildings;
using Game.State.Maps.Towers;
using UnityEngine;

namespace Game.Settings.Gameplay.Entities.Tower
{
    
    [CreateAssetMenu(fileName = "TowerSettings", 
        menuName = "Game Settings/Towers/New Tower Settings")]
    public class TowerSettings : EntitySettings<TowerLevelSettings>
    {
        [field: SerializeField] public TowerTypeDamage TypeDamage { get; private set; }
        [field: SerializeField] public TowerTypeEnemy TypeEnemy { get; private set; }

        //TODO Базовые настройки
    }
}