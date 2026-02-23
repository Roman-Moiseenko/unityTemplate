using System;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.GameplayStates;
using Game.GamePlay.Services;
using Game.Settings;
using Game.State.Gameplay;
using Game.State.Inventory;
using Game.State.Maps.Mobs;
using Game.State.Maps.Towers;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.UI.PanelBuild
{
    public class CardViewModel
    {
        public string Caption;
        public string Description;
        public string ImageCard;
        public string ImageBack;
        public string DescriptionBack;
        public int Level = 0;
        public int NumberModel;
        public readonly Dictionary<TowerParameterType, Vector2> InfoCardParameters = new(); //Параметры для отображения на Backend

        public MobDefence? Defence = null;
        public ReactiveProperty<bool> Updated = new(true);
        
        
        private readonly GameSettings _gameSettings;
        private RewardCardData _rewardData;
        private readonly FsmGameplay _fsmGameplay;
        private readonly TowersService _towersService;
        public RewardType RewardType => _rewardData.RewardType; 
        
        public readonly Dictionary<TowerParameterType, float> UpgradeParameters = new(); //Параметры которые увеличиваются на текущем уровне

        public CardViewModel(GameSettings gameSettings, FsmGameplay fsmGameplay, TowersService towersService)
        {
            _gameSettings = gameSettings;
            _fsmGameplay = fsmGameplay;
            _towersService = towersService;
        }


        public void UpdateRewardInfo(RewardCardData rewardData)
        {
            _rewardData = rewardData;
            InfoCardParameters.Clear();
            switch (rewardData.RewardType)
            {
                case RewardType.Tower: InfoTower(); break;
                case RewardType.Ground: InfoGround(); break;
                case RewardType.Road: InfoRoad(); break;
                case RewardType.TowerLevelUp: InfoTowerUp(); break;
                case RewardType.SkillLevelUp:
                            
                    break;
                case RewardType.HeroLevelUp:
                         
                    break;
                case RewardType.TowerMove:
                            
                    break;
                case RewardType.TowerReplace:
                            
                    break;
                default: throw new Exception("Не известное значение");
            }
        }
    
        private void InfoTowerUp()
        {
            var config = _gameSettings.TowersSettings.AllTowers.Find(t => t.ConfigId == _rewardData.ConfigId);
            Level = _rewardData.Level;
            
            Caption= "УЛУЧШЕНИЕ";
            DescriptionBack = Caption;
            
            Defence = null;
            ImageCard = _rewardData.ConfigId;
            ImageBack = "TowerUpCard";

            List<TowerParameterType> listUpgradeParameter = new(); //Временный список всех параметров по всем геймплей уровням
            foreach (var configGameplayLevel in config.GameplayLevels)
            {
                foreach (var param in configGameplayLevel.Parameters)
                {
                    listUpgradeParameter.Add(param.ParameterType);
                }
            }
            List<TowerParameterType> allUpgradeParameter = listUpgradeParameter.Distinct().ToList(); //Уникальный список
            
            //Параметры upgrade текущего gameplay
            var gameplayParameters = config.GameplayLevels.Find(v => v.Level == Level + 1).Parameters;
            
            //Список на Frontend параметров, которые вырастут
            UpgradeParameters.Clear();
            foreach (var parameter in gameplayParameters)
                UpgradeParameters.Add(parameter.ParameterType, parameter.Value);

            InfoCardParameters.Clear();
            var settingsParameters = _towersService.TowerParametersMap[_rewardData.ConfigId];
            foreach (var parameterType in allUpgradeParameter)
            {
                var data = new Vector2();

                //Основная характеристика
                if (settingsParameters.TryGetValue(parameterType, out var value))
                    data.x = value.Value;
                
                var  towerParameter = gameplayParameters.Find(p => p.ParameterType == parameterType);
                //Процент роста
                if (towerParameter != null)
                    data.y = towerParameter.Value;
                
                InfoCardParameters.Add(parameterType, data);
            }
            
            Updated.OnNext(true);
        }

        private void InfoRoad()
        {
            Caption = "";
            Level = 0;
            Defence = null;
            ImageCard = _rewardData.ConfigId;
            ImageBack = "CardBuild";
            var text = _rewardData.ConfigId switch
            {
                "0" or "1" => "1X1",
                "8" => "2X2",
                _ => "1X2"
            };
            Description = text + " \nПУТЬ";
            DescriptionBack = "РАСШИРЯЕТ ДОРОГУ";
            Updated.OnNext(true);
        }

        private void InfoGround()
        {
            Caption = "";
            Level = 0;
            Defence = null;
            ImageCard = "CardGround";
            ImageBack = "CardBuild";
            Description = "ДОП.ПОЛЕ";
            DescriptionBack = "РАСШИРЯЕТ ТЕРРИТОРИЮ";
            Updated.OnNext(true);
        }

        private void InfoTower()
        {
            var config = _gameSettings.TowersSettings.AllTowers
                .Find(t => t.ConfigId == _rewardData.ConfigId);
            Caption = "";
            Level = _rewardData.Level;
            NumberModel = _rewardData.Level switch
            {
                1 or 2 => 1,
                3 or 4 => 2,
                5 or 6 => 3,
                _ => throw new Exception("Неизвестный уровень")
            };
            Description = "БАШНЯ \n" + config.TitleLid;
            DescriptionBack = Description;
            Defence = config.Defence;
            ImageCard = _rewardData.ConfigId;
            ImageBack = _rewardData.EpicLevel.Index().ToString();
            InfoCardParameters.Clear();
            foreach (var parameterData in _towersService.TowerParametersMap[_rewardData.ConfigId])
            {
                InfoCardParameters.Add(parameterData.Key, new Vector2(parameterData.Value.Value, 0));
            }
            
            Updated.OnNext(true);
        }
        
        
        public void RequestBuild()
        {
            if (_rewardData.IsBuild())
            {
                _fsmGameplay.Fsm.SetState<FsmStateBuild>(_rewardData);
            }
            else //Завершение строительства
            {
                _fsmGameplay.Fsm.SetState<FsmStateBuildEnd>(_rewardData);
            }
        }
    }
}