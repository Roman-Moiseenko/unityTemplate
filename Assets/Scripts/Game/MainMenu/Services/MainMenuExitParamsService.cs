using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.GamePlay.Classes;
using Game.GamePlay.Root;
using Game.GameRoot.Queries.HeroQueries;
using Game.MainMenu.Root;
using Game.Settings;
using Game.Settings.Gameplay.Entities.Heroes;
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
using MVVM.CMD;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.MainMenu.Services
{
    public class MainMenuExitParamsService
    {
        private readonly DIContainer _container;
        private readonly GameStateProxy _gameState;
        private GameplayEnterParams _gameplayEnterParams;
        private readonly IQueryProcessor _qrc;

        public MainMenuExitParamsService(DIContainer container)
        {
            _container = container;
            _gameState = container.Resolve<IGameStateProvider>().GameState;
            _qrc = container.Resolve<IQueryProcessor>();
        }

        public MainMenuExitParams GetExitParams(TypeGameplay typeGameplay, int currentIdMap)
        {
            _gameplayEnterParams = new GameplayEnterParams(typeGameplay, currentIdMap);
            var gameState = _container.Resolve<IGameStateProvider>().GameState;
            var deckCard = gameState.Inventory.GetCurrentDeckCard();

            //Переносим башни
            foreach (var towerUniqueId in deckCard.TowerCardIds)
            {
                //TODO Сделать копию towerCard и ее передавать
                var towerCard = gameState.Inventory.Items.FirstOrDefault(item => item.UniqueId == towerUniqueId);
                if (towerCard == null) throw new Exception($"Отсутствует в инвентаре башня с id = {towerUniqueId}");
                _gameplayEnterParams.Towers.Add((TowerCardData)towerCard?.Origin);
            }
            
            //Переносим навыки
            foreach (var skillUniqueId in deckCard.SkillCardIds)
            {
                var skillCard = gameState.Inventory.Items.FirstOrDefault(item => item.UniqueId == skillUniqueId);
                if (skillCard == null) throw new Exception($"Отсутствует в инвентаре навык с id = {skillUniqueId}");
                _gameplayEnterParams.Skills.Add((SkillCardData)skillCard?.Origin);
            }
            
            
            //Переносим героя
            //Героя ищем по ConfigId
            var heroCard = gameState.Inventory.Items.FirstOrDefault(item => item.ConfigId == deckCard.HeroConfigId.CurrentValue);
            
            if (heroCard == null)
                throw new Exception($"Отсутствует в инвентаре герой с id = {deckCard.HeroConfigId.CurrentValue}");
            
            _gameplayEnterParams.HeroCard = (HeroCardData)heroCard.Origin;

            //TODO Бустери из науки
            
            _gameplayEnterParams.GameplayBoosters = new GameplayBoosters();
            //Переносим бустеры от героя 
            var query = new QueryInfoHero(heroCard.ConfigId);
            var heroSettings = _qrc.Request<QueryInfoHero, HeroSettings>(query); 
            
            var currentRank = ((HeroCard)heroCard).Rank.CurrentValue;
            var currentLevel = ((HeroCard)heroCard).Level.CurrentValue;
            //Бустеры от Уровня
            foreach (var entityParameter in heroSettings.LevelEntityParameters)
            {
                if (entityParameter.Index <= currentLevel) AddBuster(entityParameter);
            }
            //Бустеры от ранга
            foreach (var entityParameter in heroSettings.RankEntityParameters)
            {
                if (entityParameter.Index <= currentRank)AddBuster(entityParameter);
            }

            /*
            Dictionary<ParameterType, float> boost = new();
            boost.Add(ParameterType.Damage, 2);
            boost.Add(ParameterType.Critical, 2);
            _gameplayEnterParams.GameplayBoosters.TowerDefenceBust.Add(TypeDefence.Advanced, boost);
            */


            //TODO Передаем сохраненные настройки геймплея, скорость не передается 
            
            _gameplayEnterParams.GameSpeed = _gameState.GameSpeed.CurrentValue;
            
            var mainMenuExitParams = new MainMenuExitParams(_gameplayEnterParams);

            return mainMenuExitParams;
        }

        private void AddBuster(EntityParameter parameter)
        {
            //Бустеры для Героя
            if (parameter.Entity == TypeEntity.Hero)
            {
                if (_gameplayEnterParams.GameplayBoosters.HeroBust.TryGetValue(parameter.Parameter, out _))
                {
                    _gameplayEnterParams.GameplayBoosters.HeroBust[parameter.Parameter] += parameter.Value;
                }
                else
                {
                    _gameplayEnterParams.GameplayBoosters.HeroBust.Add(parameter.Parameter, parameter.Value);
                }
            }

            //Бустеры для Башен
            if (parameter.Entity == TypeEntity.Tower)
            {
                if (parameter.Defence == TypeDefenceAdvanced.All) //Для всех типов башен
                {
                    if (_gameplayEnterParams.GameplayBoosters.TowerBust.TryGetValue(parameter.Parameter, out _))
                    {
                        _gameplayEnterParams.GameplayBoosters.TowerBust[parameter.Parameter] += parameter.Value;
                    }
                    else
                    {
                        _gameplayEnterParams.GameplayBoosters.TowerBust.Add(parameter.Parameter, parameter.Value);
                    }
                }
                else //Для башен по типу Defence
                {
                    var defence = parameter.Defence.ToDefence(); //Переводим в тип TypeDefence из TypeDefenceAdvanced

                    if (_gameplayEnterParams.GameplayBoosters.TowerDefenceBust.TryGetValue(defence,
                            out var paramValueTower))
                    {

                        if (paramValueTower.TryGetValue(parameter.Parameter, out _))
                        {
                            paramValueTower[parameter.Parameter] += parameter.Value;
                        }
                        else
                        {
                            paramValueTower.Add(parameter.Parameter, parameter.Value);
                        }
                        
                    }
                    else
                    {
                        Dictionary<ParameterType, float> boost = new()
                        {
                            { parameter.Parameter, parameter.Value }
                        };
                        _gameplayEnterParams.GameplayBoosters.TowerDefenceBust.Add(defence, boost);
                    }
                }
                
                
            }
            
            //Бустеры для Навыков (по аналогии с Башнями)
            if (parameter.Entity == TypeEntity.Skill)
            {
                if (parameter.Defence == TypeDefenceAdvanced.All) //
                {
                    if (_gameplayEnterParams.GameplayBoosters.SkillBust.TryGetValue(parameter.Parameter, out _))
                    {
                        _gameplayEnterParams.GameplayBoosters.SkillBust[parameter.Parameter] += parameter.Value;
                    }
                    else
                    {
                        _gameplayEnterParams.GameplayBoosters.SkillBust.Add(parameter.Parameter, parameter.Value);
                    }
                }
                else
                {
                    var defence = parameter.Defence.ToDefence(); //Переводим в тип TypeDefence из TypeDefenceAdvanced

                    if (_gameplayEnterParams.GameplayBoosters.SkillDefenceBust.TryGetValue(defence,
                            out var paramValueSkill))
                    {

                        if (paramValueSkill.TryGetValue(parameter.Parameter, out _))
                        {
                            paramValueSkill[parameter.Parameter] += parameter.Value;
                        }
                        else
                        {
                            paramValueSkill.Add(parameter.Parameter, parameter.Value);
                        }
                        
                    }
                    else
                    {
                        Dictionary<ParameterType, float> boost = new()
                        {
                            { parameter.Parameter, parameter.Value }
                        };
                        _gameplayEnterParams.GameplayBoosters.SkillDefenceBust.Add(defence, boost);
                    }
                }
                
                
            }
        }
    }
}