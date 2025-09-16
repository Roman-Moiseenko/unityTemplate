using System;
using DI;
using Game.GamePlay.Classes;
using Game.MainMenu.Commands.TowerCommands;
using Game.MainMenu.Root;
using Game.State.Inventory;
using Game.State.Maps.Rewards;
using Game.State.Root;
using MVVM.CMD;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.MainMenu.Services
{
    public class InventoryService
    {
        private readonly ICommandProcessor _cmd;
        private readonly GameStateProxy _gameState;
        private readonly ChestService _chestService;
        private readonly DIContainer _container;

        public InventoryService(ICommandProcessor cmd, GameStateProxy gameState, ChestService chestService)
        {
            _cmd = cmd;
            _gameState = gameState;
            _chestService = chestService;
        }

        /**
         * Обрабатываем входящие параметры после геймплея - Бесконечный бой
         */
        public void InfinityRewardGamePlay(MainMenuEnterParams enterParams)
        {
            Debug.Log(" MainMenuEnterParams " + JsonConvert.SerializeObject(enterParams, Formatting.Indented));
            //Монетки
            _gameState.SoftCurrency.Value += enterParams.SoftCurrency;
            //Карты и чертежи (итемсы)
            enterParams.RewardCards.ForEach(RewardToItem);
            
            enterParams.TypeChest = _chestService.AddChestInfinity(enterParams.LastWave);
        }
        
        
        public void RewardToItem(RewardEntityData rewardEntityData)
        {
            switch (rewardEntityData.RewardType)
            {
                case InventoryType.TowerCard:
                    var commandTowerCard = new CommandTowerCardAdd
                    {
                        ConfigId = rewardEntityData.ConfigId,
                        Level = 1,
                        EpicLevel = TypeEpicCard.Normal
                    };
                    _cmd.Process(commandTowerCard);
                    break;
                case InventoryType.TowerPlan:
                    var commandTowerPlan = new CommandTowerPlanAdd
                    {
                        ConfigId = rewardEntityData.ConfigId,
                        Amount = 1,

                    };
                    _cmd.Process(commandTowerPlan);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        //TODO Сервис работы с инвентарем
        
        //
    }
}