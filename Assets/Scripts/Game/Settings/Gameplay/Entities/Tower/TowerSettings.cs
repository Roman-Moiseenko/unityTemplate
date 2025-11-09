using System.Collections.Generic;
using Game.State.Maps.Mobs;
using Game.State.Maps.Towers;
using UnityEngine;

namespace Game.Settings.Gameplay.Entities.Tower
{
    public class TowerSettings
    {
        public string ConfigId;
        public string TitleLid;
        public string DescriptionLid;
        public string PrefabPath;
        public bool OnRoad;
        public bool MultiShot; //Массовый урон всем в области действия
        public TowerTypeEnemy TypeEnemy;

        public MobDefence Defence;

        //  public string PrefabName { get; private set; }
        public ShotSettings Shot = new();
        public int AvailableWave;
        
        //Характеристики, которые зависят от уровня (звездочек) в гейплее - не влияют на карточку башни
        public List<TowerLevelSettings> GameplayLevels = new();

        //Базовые значения всех доступных характеристик для карты башни
        public List<TowerParameterSettings> BaseParameters = new();
        // [field: SerializeField] public List<TowerEpicLevelCard> EpicLevels { get; private set; } = new();
        
        //Характеристики, которые растут от уровня карты башни, скорость роста зависит от Эпичности
        public List<LevelCardParameters> LevelCardParameters = new();
        
        //Характеристики, которые растут только от Эпичности карты башни
        public List<EpicCardParameters> EpicCardParameters = new();

        //Настройки выстрела
    }
}