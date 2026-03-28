using System.Collections.Generic;
using Game.State.Common;

namespace Game.Settings.Gameplay.Entities.Skill
{
    public class SkillSettings
    {
        public string ConfigId;
        public string TitleLid;
        public string DescriptionLid;
        public string PrefabPath;
        public TypeTarget TypeTarget;
        public List<SkillParameterSettings> BaseParameters { get; set; }

        public TypeDefence Defence { get; set; }

        //Характеристики, которые зависят от уровня (звездочек) в гейплее - не влияют на карточку башни
        public List<SkillLevelSettings> GameplayLevels = new();

        //Базовые значения всех доступных характеристик для карты башни
        // [field: SerializeField] public List<TowerEpicLevelCard> EpicLevels { get; private set; } = new();
        
        //Характеристики, которые растут от уровня карты башни, скорость роста зависит от Эпичности
        public List<LevelCardParameters> LevelCardParameters = new();
        
        //Характеристики, которые растут только от Эпичности карты башни
        public List<EpicCardParameters> EpicCardParameters = new();    }
}