using System;
using System.Collections.Generic;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.GameplayStates;
using Game.GamePlay.Services;
using Game.GameRoot.Services;
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
        public Dictionary<TowerParameterType, Vector2> Parameters = new();

        public MobDefence? Defence = null;
        public ReactiveProperty<bool> Updated = new(true);
        
        
        public GameSettings _gameSettings;
        private RewardCardData _rewardData;
        private readonly FsmGameplay _fsmGameplay;
        private readonly TowersService _towersService;
        public RewardType RewardType => _rewardData.RewardType; 

        //private readonly ResourceService _resourceService;

        public CardViewModel(GameSettings gameSettings, FsmGameplay fsmGameplay, TowersService towersService)
        {
          //  _resourceService = resourceService;
            _gameSettings = gameSettings;
            _fsmGameplay = fsmGameplay;
            _towersService = towersService;
        }


        public void UpdateRewardInfo(RewardCardData rewardData)
        {
            _rewardData = rewardData;
            Parameters.Clear();
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

            var parameters = config.GameplayLevels.Find(v => v.Level == Level + 1);
            Description = "";
            foreach (var parameter in parameters.Parameters)
            {
                //Debug.Log($"{parameter.ParameterType.GetString()} {parameter.Value} %\n");
                Description += $"{parameter.ParameterType.GetString()} {parameter.Value} %\n";
            }
            
            foreach (var parameterData in _towersService.TowerParametersMap[_rewardData.ConfigId])
            {
                var data = new Vector2(parameterData.Value.Value, 0);
                var paramUpdate = parameters.Parameters.Find(t => t.ParameterType == parameterData.Key);
                
                if (paramUpdate != null) data.y = parameterData.Value.Value;
                
                Parameters.Add(parameterData.Key, data);
            }
            
            Updated.OnNext(true);
          //  Defence.OnNext(null);
        }

        private void InfoRoad()
        {
            Caption = "";
            Level = 0;
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
            foreach (var parameterData in _towersService.TowerParametersMap[_rewardData.ConfigId])
            {
                Parameters.Add(parameterData.Key, new Vector2(parameterData.Value.Value, 0));
            }
            
            Updated.OnNext(true);
        }

        
        
        public void RequestBuild()
        {
            if (_rewardData.IsBuild())
            {
                _fsmGameplay.Fsm.SetState<FsmStateBuild>(_rewardData);
//                Debug.Log("Режим строительства = " + _rewardData.ConfigId);
            }
            else
            {
                //Завершение строительства
                _fsmGameplay.Fsm.SetState<FsmStateBuildEnd>(_rewardData);
                //Необходимо дождаться проигрывания анимации
//                _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
//                Debug.Log("Режим конца строительства = " + _rewardData.ConfigId);
            }
        }
    }
}