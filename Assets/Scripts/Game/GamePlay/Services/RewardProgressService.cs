using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.State;
using Game.State.Gameplay;
using Game.State.Root;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class RewardProgressService
    {
        private readonly DIContainer _container;
        private readonly GameplayStateProxy _gameplayStateProxy;

        public RewardProgressService(DIContainer container)
        {
            _container = container;
            _gameplayStateProxy = container.Resolve<IGameStateProvider>().GameplayState;
            var fsm = container.Resolve<FsmGameplay>();

            _gameplayStateProxy.Progress.Subscribe(newValue =>
            {
                if (newValue >= 100)
                {
                    var rewards = GenerateReward();  //1. Создаем награды
                    fsm.Fsm.SetState<FsmStateBuildBegin>(rewards);
                }
            });

            //TODO Куда перенести
            fsm.Fsm.StateCurrent.Subscribe(newState =>
            {
                
            });

        }

        private void SetReward()
        {
            
        }

        private RewardsProgress GenerateReward()
        {
            //TODO Генерация награды, в зависимости от
            //1. Какие карточки в инвентаре
            //2. Какие башни на карте
            //3. Какой прогресс, при ProgressLevel == 0, все три карты - башни

            var rewards = new RewardsProgress();
            rewards.Card1.RewardType = RewardType.Tower;
            rewards.Card1.ConfigId = "Tower01";

            rewards.Card2.RewardType = RewardType.TowerBust;
            rewards.Card2.TargetId = "Tower01";
            rewards.Card2.ConfigId = "Damage";
            
            rewards.Card3.RewardType = RewardType.Ground;
            rewards.Card3.ConfigId = "Glass01"; //TODO Брать из текущих настроек карты
            
            return rewards;
        }
        
    }
}