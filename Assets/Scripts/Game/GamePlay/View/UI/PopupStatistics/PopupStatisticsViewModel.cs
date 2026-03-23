using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.GamePlay.Services;
using Game.Settings;
using Game.State;
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
        public List<StatisticElementViewModel> Elements = new();
        public int AllDamage;
        public PopupStatisticsViewModel(DIContainer container) : base(container)
        {
            var gameplayState = container.Resolve<IGameStateProvider>().GameplayState;
            var statisticGame = gameplayState.StatisticGame;
            var gameSettings = container.Resolve<ISettingsProvider>().GameSettings;
            var towersService = container.Resolve<TowersService>();
            var towersSettings = gameSettings.TowersSettings.AllTowers;
            
            AllDamage = Mathf.RoundToInt(statisticGame.AllDamage.CurrentValue);            
            
            //TODO var skillsSettings = gameSettings.TowersSettings.AllSkills;
            //TODO var heroesSettings = gameSettings.TowersSettings.AllHeroes;
            
            //var towerSetting = towersSettings.FirstOrDefault(t => t.ConfigId == tower.Key);
            foreach (var entityDamage in statisticGame.GetDamages())
            {
                var stat = new StatisticElementViewModel(container)
                {
                    ConfigId = entityDamage.ConfigId,
                    Damage = entityDamage.Damage,
                    Percent = entityDamage.Damage / statisticGame.AllDamage.CurrentValue * 100f,
                    Count = statisticGame.GetTowerCount(entityDamage.ConfigId),
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