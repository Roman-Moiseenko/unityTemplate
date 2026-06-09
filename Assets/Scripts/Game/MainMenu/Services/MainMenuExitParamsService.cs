using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.GamePlay.Classes;
using Game.GamePlay.Root;
using Game.MainMenu.Root;
using Game.State;
using Game.State.Common;
using Game.State.Inventory.HeroCards;
using Game.State.Inventory.SkillCards;
using Game.State.Inventory.TowerCards;
using Game.State.Maps.Mobs;
using Game.State.Maps.Towers;
using Game.State.Parameters;
using Game.State.Research;
using Game.State.Root;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.MainMenu.Services
{
    public class MainMenuExitParamsService
    {
        private readonly DIContainer _container;
        private readonly GameStateProxy _gameState;

        public MainMenuExitParamsService(DIContainer container)
        {
            _container = container;
            _gameState = container.Resolve<IGameStateProvider>().GameState;
        }

        public MainMenuExitParams GetExitParams(TypeGameplay typeGameplay, int currentIdMap)
        {
            var gameplayEnterParams = new GameplayEnterParams(typeGameplay, currentIdMap);
            var gameState = _container.Resolve<IGameStateProvider>().GameState;
            var deckCard = gameState.Inventory.GetCurrentDeckCard();

            //Переносим башни
            foreach (var towerUniqueId in deckCard.TowerCardIds)
            {
                //TODO Сделать копию towerCard и ее передавать
                var towerCard = gameState.Inventory.Items.FirstOrDefault(item => item.UniqueId == towerUniqueId);
                if (towerCard == null) throw new Exception($"Отсутствует в инвентаре башня с id = {towerUniqueId}");
                gameplayEnterParams.Towers.Add((TowerCardData)towerCard?.Origin);
            }
            
            //Переносим навыки
            foreach (var skillUniqueId in deckCard.SkillCardIds)
            {
                var skillCard = gameState.Inventory.Items.FirstOrDefault(item => item.UniqueId == skillUniqueId);
                if (skillCard == null) throw new Exception($"Отсутствует в инвентаре навык с id = {skillUniqueId}");
                gameplayEnterParams.Skills.Add((SkillCardData)skillCard?.Origin);
            }
            
            
            //TODO Переносим героя
            
            var heroCard = gameState.Inventory.Items.FirstOrDefault(item => item.UniqueId == deckCard.HeroCardId.CurrentValue);
            if (heroCard == null)
            {
                //MAINDO Исключение throw new Exception($"Отсутствует в инвентаре герой с id = {deckCard.HeroCardId.CurrentValue}");
                
                var heroCardData = new HeroCardData
                {
                    Defence = TypeDefence.Advanced,
                    EpicLevel = TypeEpic.Good,
                    Level = 1,
                    Rank = 1,
                    Available = true,
                    ConfigId = "Hero00",
                    UniqueId = 9999999,
                    Amount = 1,
                    Name = "Герой"
                };
                heroCardData.Parameters.Add(ParameterType.Damage, new ParameterData
                {
                    ParameterType = ParameterType.Damage,
                    Value = 100,
                });
                heroCardData.Parameters.Add(ParameterType.Critical, new ParameterData
                {
                    ParameterType = ParameterType.Critical,
                    Value = 10,
                });
                heroCardData.Parameters.Add(ParameterType.Speed, new ParameterData
                {
                    ParameterType = ParameterType.Speed,
                    Value = 1,
                });
                heroCard = new HeroCard(heroCardData);
            }

            gameplayEnterParams.HeroCard = (HeroCardData)heroCard.Origin;

            //Переносим коэффициенты из науки

            //TODO Перенести данные из сервиса, когда он появится
            gameplayEnterParams.GameplayBoosters = new GameplayBoosters()
            {
                TowerDamage = 1f,
                RewardCurrency = 1.5f,
                SkillDamage = 1f,
                //TowerDistance = 10,
            };

            Dictionary<ParameterType, float> boost = new();
            boost.Add(ParameterType.Damage, 2);
            boost.Add(ParameterType.Critical, 2);
            gameplayEnterParams.GameplayBoosters.TowerDefenceBust.Add(TypeDefence.Advanced, boost);
            


            //TODO Передаем сохраненные настройки геймплея, скорость не передается 
            
            gameplayEnterParams.GameSpeed = _gameState.GameSpeed.CurrentValue;


            var mainMenuExitParams = new MainMenuExitParams(gameplayEnterParams);

            return mainMenuExitParams;
        }
    }
}