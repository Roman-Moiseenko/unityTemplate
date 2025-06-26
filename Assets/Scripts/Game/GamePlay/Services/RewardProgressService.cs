using System;
using DI;
using Game.GamePlay.Commands.TowerCommand;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.State;
using Game.State.Gameplay;
using Game.State.Root;
using MVVM.CMD;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.GamePlay.Services
{
    public class RewardProgressService
    {
        private readonly DIContainer _container;
        private readonly GameplayStateProxy _gameplayStateProxy;
        private readonly TowersService _towerService;
        private readonly GroundsService _groundService;

        public RewardProgressService(DIContainer container)
        {
            _container = container;
            //Сервисы для наград
            _towerService = container.Resolve<TowersService>();
            _groundService = container.Resolve<GroundsService>();

            _gameplayStateProxy = container.Resolve<IGameStateProvider>().GameplayState;
            var fsm = container.Resolve<FsmGameplay>();

            _gameplayStateProxy.Progress.Subscribe(newValue =>
            {
                if (newValue >= 100)
                {
                    var rewards = GenerateReward(); //1. Создаем награды
                    fsm.Fsm.SetState<FsmStateBuildBegin>(rewards);
                }
            });

            //TODO Куда перенести
            fsm.Fsm.StateCurrent.Subscribe(newState =>
            {
                if (newState.GetType() == typeof(FsmStateBuild))
                {
                    //TODO Имитируем работу контроллера ввода
                    /*
                    var card = ((FsmStateBuild)newState).GetRewardCard();
                    card.Direction = 2;
                    card.Position = new Vector2Int(Random.Range(0, 5), Random.Range(0, 2));
                    fsm.Fsm.SetState<FsmStateBuildEnd>(card); */
                    //
                }

                if (newState.GetType() == typeof(FsmStateBuildEnd))
                {
                    var card = ((FsmStateBuildEnd)newState).GetRewardCard();
                    _gameplayStateProxy.Progress.Value -= 100;
                    _gameplayStateProxy.ProgressLevel.Value++;
                    //   SetReward(card);
                }
            });
        }

        /**
         * По типу награды, который вернулся от игрока запускаем метод сервиса, передав данные
         */
        /*  private void SetReward(RewardCardData card)
          {
              switch (card.RewardType)
              {
                  //case RewardType.Tower: _towerService.PlaceTower(card.ConfigId, card.Position); break;
                  case RewardType.Ground: _groundService.PlaceGround(card.Position); break;
                  case RewardType.Road: Debug.Log("Размещение дороги. В разработке"); break;
                  case RewardType.TowerBust: _towerService.ApplyBust(card.TargetId, card.ConfigId); break;
                  case RewardType.TowerMove: _towerService.MoveTower(card.UniqueId, card.Position); break;
                  case RewardType.TowerReplace: _towerService.ReplaceTower(card.UniqueId, card.UniqueId2); break;
                  case RewardType.SkillBust: Debug.Log("Усиление навыка. В разработке"); break;
                  case RewardType.HeroBust: Debug.Log("Усиление героя. В разработке"); break;
                  default: throw new Exception($"Неверный тип награды {card.RewardType}");
              }

          }
  */
        private RewardsProgress GenerateReward()
        {
            //TODO Генерация награды, в зависимости от
            //1. Какие карточки в инвентаре
            //2. Какие башни на карте
            //3. Какой прогресс, при ProgressLevel == 0, все три карты - башни

            var rewards = new RewardsProgress();
            rewards.Card1.RewardType = RewardType.Tower;
            rewards.Card1.ConfigId = "Tower02";

            rewards.Card2.RewardType = RewardType.Ground;
            //rewards.Card2.ConfigId = "Tower01";

            rewards.Card3.RewardType = RewardType.Road;
            //rewards.Card3.ConfigId = "5";
            
            var number = Mathf.FloorToInt(Mathf.Abs(Random.insideUnitSphere.x) * 999);
            rewards.Card3.ConfigId = (number % 9).ToString();
            
            return rewards;
        }
    }
}