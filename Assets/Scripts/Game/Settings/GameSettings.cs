using System;
using System.Collections.Generic;
using System.Linq;
using Game.Settings.Gameplay.Enemies;
using Game.Settings.Gameplay.Entities.Skill;
using Game.Settings.Gameplay.Entities.Tower;
using Game.Settings.Gameplay.Initial;
using Game.Settings.Gameplay.Maps;

namespace Game.Settings
{
    public class GameSettings
    {
       public MapsSettings MapsSettings = new();
       public TowersSettings TowersSettings = new();
       public SkillsSettings SkillsSettings = new();
       public CastleInitialSettings CastleInitialSettings = new();
       public InventoryInitialSettings InventoryInitialSettings = new();
       public MobsSettings MobsSettings = new();
       public DateTime DateVersion = new();


       //public List<ParameterDefinition> ParameterDefinitions = new();


       //[NonSerialized]
       //public Dictionary<string, ParameterDefinition> ParameterDefinitionMap;
    }
}