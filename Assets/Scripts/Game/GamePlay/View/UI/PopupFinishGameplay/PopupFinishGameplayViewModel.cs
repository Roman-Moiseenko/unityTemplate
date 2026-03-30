using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.GamePlay.Root;
using Game.GamePlay.Services;
using Game.GamePlay.View.UI.PopupStatistics;
using Game.GameRoot.View.ResourceReward;
using Game.MainMenu.Root;
using Game.Settings;
using Game.State;
using Game.State.Gameplay.Statistics;
using Game.State.Inventory;
using Game.State.Inventory.Chests;
using Game.State.Inventory.Common;
using Game.State.Maps.Rewards;
using MVVM.UI;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.UI.PopupFinishGameplay
{
    public class PopupFinishGameplayViewModel : WindowViewModel
    {
        public readonly MainMenuEnterParams EnterParams;

        public TypeChest RewardChest;
        public override string Id => "PopupFinishGameplay";
        public override string Path => "Gameplay/Popups/";
        public List<ResourceRewardViewModel> RewardResources = new();
        public int MapId;
        public StatisticGame StatisticGame;
        public float TotalTimeInScene;
        private readonly List<RewardEntityData> _rewards = new();
        private readonly GameplayExitParams _exitParams;
        private readonly Subject<GameplayExitParams> _exitSceneRequest;
        
        public List<StatisticElementViewModel> Elements = new();
        public int AllDamage;
        public PopupFinishGameplayViewModel(
            GameplayExitParams exitParams, 
            Subject<GameplayExitParams> exitSceneRequest,
            DIContainer container) : base(container)
        {
            var gameplayState = container.Resolve<IGameStateProvider>().GameplayState;
            MapId = gameplayState.MapId.CurrentValue;
            TotalTimeInScene = gameplayState.TotalTimeInScene.CurrentValue;
            StatisticGame = gameplayState.StatisticGame;
            EnterParams = exitParams.MainMenuEnterParams;
            _exitParams = exitParams;
            _exitSceneRequest = exitSceneRequest;
            
           // var gameState = container.Resolve<IGameStateProvider>().GameState;

            _rewards.Add(new RewardEntityData
            {
                RewardType = InventoryType.SoftCurrency,
                ConfigId = "Currency",
                Amount = EnterParams.SoftCurrency,
            });

            foreach (var rewardCard in EnterParams.RewardCards)
            {
                var element = _rewards.Find(v =>
                    v.RewardType == rewardCard.RewardType &&
                    v.ConfigId == rewardCard.ConfigId);
                if (element != null)
                {
                    element.Amount += rewardCard.Amount;
                }
                else
                {
                    _rewards.Add(rewardCard);
                }
            }

            foreach (var rewardEntity in EnterParams.RewardOnWave)
            {
                _rewards.Add(rewardEntity);
            }

            foreach (var reward in _rewards)
            {
                var viewModel = new ResourceRewardViewModel(reward);
                RewardResources.Add(viewModel);
            }
            
            RewardChest = EnterParams.TypeChest;
            //Статистика
            var towersService = container.Resolve<TowersService>();
            var statisticGame = gameplayState.StatisticGame;
            var gameSettings = container.Resolve<ISettingsProvider>().GameSettings;
            var towersSettings = gameSettings.TowersSettings.AllTowers;
            
            
            AllDamage = Mathf.RoundToInt(statisticGame.AllDamage.CurrentValue); 
            
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
        public override void RequestClose()
        {
            base.RequestClose();
            _exitSceneRequest.OnNext(_exitParams);
        }
        
        private void RewardLoad(List<RewardEntityData> list)
        {
        }
    }
}