using System;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
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
        public int Level = 0;

        public MobDefence? Defence = null;

        public ReactiveProperty<bool> Updated = new(true);
        
        public GameSettings _gameSettings;
        private RewardCardData _rewardData;
        private readonly FsmGameplay _fsmGameplay;
        public RewardType RewardType => _rewardData.RewardType; 

        //private readonly ResourceService _resourceService;

        public CardViewModel(GameSettings gameSettings, FsmGameplay fsmGameplay)
        {
          //  _resourceService = resourceService;
            _gameSettings = gameSettings;
            _fsmGameplay = fsmGameplay;
        }


        public void UpdateRewardInfo(RewardCardData rewardData)
        {
            _rewardData = rewardData;
            
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
            Defence = config.Defence;
            ImageCard = _rewardData.ConfigId;
            ImageBack = "TowerUpCard";

            var parameters = config.GameplayLevels.Find(v => v.Level == Level + 1);
            Description = "";
            foreach (var parameter in parameters.Parameters)
            {
                //Debug.Log($"{parameter.ParameterType.GetString()} {parameter.Value} %\n");
                Description += $"{parameter.ParameterType.GetString()} {parameter.Value} %\n";
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
            Description = _rewardData.Name + " \nПУТЬ";
            
            Updated.OnNext(true);
        }

        private void InfoGround()
        {
            Caption = "";
            Level = 0;
            
            ImageCard = "CardGround";
            ImageBack = "CardBuild";
            Description = "ДОП.ПОЛЕ";
            Updated.OnNext(true);
        }

        private void InfoTower()
        {
            var config = _gameSettings.TowersSettings.AllTowers.Find(t => t.ConfigId == _rewardData.ConfigId);
            Caption = "";
            Level = _rewardData.Level;
            Description = "БАШНЯ \n" + config.TitleLid;
            Defence = config.Defence;
            
            ImageCard = _rewardData.ConfigId;
            ImageBack = _rewardData.EpicLevel.Index().ToString();
            
            Updated.OnNext(true);
        }

        
        
        public void RequestBuild()
        {
            if (_rewardData.IsBuild())
            {
                _fsmGameplay.Fsm.SetState<FsmStateBuild>(_rewardData);
            }
            else
            {
                _fsmGameplay.Fsm.SetState<FsmStateBuildEnd>(_rewardData);
            }
        }
    }
}