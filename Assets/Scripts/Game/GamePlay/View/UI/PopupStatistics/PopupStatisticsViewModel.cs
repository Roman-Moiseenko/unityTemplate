using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.GamePlay.Services;
using Game.Settings;
using Game.State;
using Game.State.Gameplay;
using Game.State.Gameplay.Statistics;
using Game.State.Inventory;
using MVVM.UI;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.GamePlay.View.UI.PopupStatistics
{
    public class PopupStatisticsViewModel : WindowViewModel
    {
        public override string Id => "PopupStatistics";
        public override string Path => "Gameplay/Popups/";
        public GameplayStateProxy GameplayState;
        private StatisticGame _statisticGame ;
        public List<StatisticElementViewModel> Elements = new();
        public int AllDamage;
        public PopupStatisticsViewModel(DIContainer container) : base(container)
        {
            GameplayState = container.Resolve<IGameStateProvider>().GameplayState;
            _statisticGame = GameplayState.StatisticGame;
            AllDamage = Mathf.RoundToInt(_statisticGame.AllDamage.CurrentValue);
            var gameSettings = container.Resolve<ISettingsProvider>().GameSettings;
            var towersService = container.Resolve<TowersService>();
            var towersSettings = gameSettings.TowersSettings.AllTowers;
            
            //TODO var skillsSettings = gameSettings.TowersSettings.AllSkills;
            //TODO var heroesSettings = gameSettings.TowersSettings.AllHeroes;

            
            //var towerSetting = towersSettings.FirstOrDefault(t => t.ConfigId == tower.Key);
            Debug.Log(JsonConvert.SerializeObject(_statisticGame.Origin, Formatting.Indented));
            foreach (var entityDamage in _statisticGame.Damages)
            {
                var stat = new StatisticElementViewModel(container)
                {
                    ConfigId = entityDamage.ConfigId,
                    Damage = entityDamage.Damage,
                    Percent = entityDamage.Damage / _statisticGame.AllDamage.CurrentValue * 100f,
                    Count = _statisticGame.GetTowerCount(entityDamage.ConfigId),
                    TypeEntity = entityDamage.TypeEntity,
                };

                if (entityDamage.TypeEntity == TypeEntityStatisticDamage.Tower)
                {
                    var towerSetting = towersSettings.FirstOrDefault(t => t.ConfigId == entityDamage.ConfigId);
                    if (towerSetting == null) throw new Exception("Tower no exists");
                    
                    stat.Name = towerSetting.TitleLid;
                    stat.Defence = towerSetting.Defence;
                    stat.Level = towersService.Levels[entityDamage.ConfigId];
                    stat.EpicCard = towersService.GetAvailableTowers()[entityDamage.ConfigId];
                    stat.MaxLevel = 6;
                }

                if (entityDamage.TypeEntity == TypeEntityStatisticDamage.Skill)
                {
                  //  var skillSetting = skillsSettings.FirstOrDefault(s => s.ConfigId == entityDamage.ConfigId);
                  //  if (skillSetting == null) throw new Exception("Skill no exists");
                    // Повторить
                    
                    stat.MaxLevel = 3;

                }
                if (entityDamage.TypeEntity == TypeEntityStatisticDamage.Hero)
                {
                    ///
                    /// 
                    stat.MaxLevel = 3;
                }
                if (entityDamage.TypeEntity == TypeEntityStatisticDamage.Castle)
                {
                    stat.Name = "Замок";
                    stat.MaxLevel = 0;
                }
                Elements.Add(stat);
            }
            
        }

        
    }
}