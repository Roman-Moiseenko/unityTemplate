using System.Collections.Generic;
using Game.State.Common;

namespace Game.Settings.Gameplay.Entities.Heroes
{
    public class HeroSettings
    {
        public string ConfigId;
        public string TitleLid;
        public string DescriptionLid;
        public string PrefabPath;
        public TypeTarget TypeTarget;
        public TypeDefence Defence { get; set; }
        
        public List<ParameterSettings> BaseParameters { get; set; }
        
        //Характеристики, которые зависят от уровня (звездочек) в гейплее - не влияют на карточку башни
        public List<LevelSettings> GameplayLevels = new();

        
        //Характеристики, которые растут от уровня карты героя, скорость роста зависит от ранга
        public List<LevelCardParameters> LevelCardParameters = new();
        
        //TODO Добавить бустеры от ранга
        //public List<RankParameters> RankParameters = new();
    }
}