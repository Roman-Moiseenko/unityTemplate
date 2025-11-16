using System;
using DI;
using Game.GamePlay.Classes;
using Game.MainMenu.Commands.InventoryCommands;
using Game.MainMenu.Commands.TowerCommands;
using Game.MainMenu.Root;
using Game.State.Inventory;
using Game.State.Inventory.Chests;
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
           // Debug.Log(" MainMenuEnterParams " + JsonConvert.SerializeObject(enterParams, Formatting.Indented));
            //Монетки
            _gameState.SoftCurrency.Value += enterParams.SoftCurrency;
            //Карты и чертежи (итемсы)
            enterParams.RewardCards.ForEach(RewardToItem);
            
            var chest = _chestService.AddChestInfinity(enterParams.LastWave);
            if (chest != null)
            {
                enterParams.TypeChest = (TypeChest)_chestService.AddChestInfinity(enterParams.LastWave);
            }
            else
            {
                enterParams.NotCellChest = true;
            }
             
        }

        public void LevelsRewardGamePlay(MainMenuEnterParams enterParams)
        {
            _gameState.SoftCurrency.Value += enterParams.SoftCurrency;
            enterParams.RewardCards.ForEach(RewardToItem);
            enterParams.RewardOnWave.ForEach(RewardToItem);
            enterParams.NotCellChest = _chestService.AddChestLevel(enterParams);
        }
        
        public void RewardToItem(RewardEntityData rewardEntityData)
        {
            var command = new CommandInventoryFromReward()
            {
                InventoryType = rewardEntityData.RewardType,
                ConfigId = rewardEntityData.ConfigId,
                Amount = rewardEntityData.Amount
            };
            _cmd.Process(command);

        }
        
        //TODO Сервис работы с инвентарем
        
        //
    }
}