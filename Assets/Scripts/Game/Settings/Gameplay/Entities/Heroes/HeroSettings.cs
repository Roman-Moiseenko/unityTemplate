using System.Collections.Generic;
using Game.Settings.Gameplay.Entities.Tower;
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
        public TypeEpic Epic { get; set; }
        
        //Параметры выстрела
        public ShotSettings Shot = new();
        ///<summary>
        ///Базовые параметры 
        /// </summary>>
        public List<ParameterSettings> BaseParameters { get; set; }
        ///<summary>
        ///Характеристики, которые зависят от уровня (звездочек) в гейплее - не влияют на карточку башни
        /// </summary>>
        public List<LevelSettings> GameplayLevels = new();
        
        //Характеристики, которые растут от уровня карты героя, скорость роста зависит от ранга
        public List<LevelCardParameters> LevelCardParameters = new();
        //Характеристики, которые растут от ранга карты героя
        
        public List<RankCardParameters> RankCardParameters = new();
        
        
        
        //Бустеры от ранга (со 2го) и от левела (шаг +20)
        public List<EntityParameter> LevelEntityParameters = new();
        public List<EntityParameter> RankEntityParameters = new();
        
    }
}