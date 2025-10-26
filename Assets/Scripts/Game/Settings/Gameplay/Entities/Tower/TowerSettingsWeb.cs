using System.Collections.Generic;
using Game.State.Maps.Mobs;
using Game.State.Maps.Towers;
using UnityEngine;

namespace Game.Settings.Gameplay.Entities.Tower
{
    
    public class TowerSettingsWeb
    {
        public string ConfigId { get; private set; }
        public string TitleLid { get; private set; }
        public string DescriptionLid { get; private set; }
        public string PrefabPath { get; private set; }
        public bool OnRoad { get; private set; }
        public bool MultiShot { get; private set; } //Массовый урон всем в области действия
        public TowerTypeEnemy TypeEnemy { get; private set; }

        public MobDefence Defence { get; private set; }

        //  public string PrefabName { get; private set; }
        public ShotSettings Shot { get; private set; } = new();
        public int AvailableWave { get; private set; }
        
        //Характеристики, которые зависят от уровня (звездочек) в гейплее - не влияют на карточку башни
        public List<TowerLevelSettings> GameplayLevels { get; private set; } = new();

        //Базовые значения всех доступных характеристик для карты башни
        public List<TowerParameterSettings> BaseParameters { get; private set; } = new();
       // [field: SerializeField] public List<TowerEpicLevelCard> EpicLevels { get; private set; } = new();
        
        //Характеристики, которые растут от уровня карты башни, скорость роста зависит от Эпичности
        public List<LevelCardParameters> LevelCardParameters { get; private set; } = new();
        
        //Характеристики, которые растут только от Эпичности карты башни
        public List<EpicCardParameters> EpicCardParameters { get; private set; } = new();

        //Настройки выстрела
    }
}