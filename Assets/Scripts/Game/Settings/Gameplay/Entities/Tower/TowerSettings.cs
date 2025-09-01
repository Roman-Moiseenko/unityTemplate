using System.Collections.Generic;
using Game.Settings.Gameplay.Buildings;
using Game.State.Maps.Mobs;
using Game.State.Maps.Towers;
using UnityEngine;

namespace Game.Settings.Gameplay.Entities.Tower
{
    [CreateAssetMenu(fileName = "TowerSettings",
        menuName = "Game Settings/Towers/New Tower Settings")]
    public class TowerSettings : ScriptableObject
    {
        [field: SerializeField] public string ConfigId { get; private set; }
        [field: SerializeField] public string TitleLid { get; private set; }
        [field: SerializeField] public string DescriptionLid { get; private set; }
        [field: SerializeField] public string PrefabPath { get; private set; }
        [field: SerializeField] public bool OnRoad { get; private set; }
        [field: SerializeField] public bool MultiShot { get; private set; } //Массовый урон всем в области действия
        [field: SerializeField] public TowerTypeEnemy TypeEnemy { get; private set; }

        [field: SerializeField] public MobDefence Defence { get; private set; }

        //  [field: SerializeField] public string PrefabName { get; private set; }
        [field: SerializeField] public ShotSettings Shot { get; private set; }
        [field: SerializeField] public List<TowerLevelSettings> GameplayLevels { get; private set; }

        [field: SerializeField] public List<TowerParameterSettings> BaseParameters { get; private set; }
        [field: SerializeField] public List<TowerEpicLevelCard> EpicLevels { get; private set; }


        //Настройки выстрела
    }
}