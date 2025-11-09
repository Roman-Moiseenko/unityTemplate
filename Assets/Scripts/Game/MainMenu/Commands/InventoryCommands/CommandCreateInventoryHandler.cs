using System;
using System.Collections.Generic;
using System.Linq;
using Game.MainMenu.Commands.TowerCommands;
using Game.Settings;
using Game.State.Inventory;
using Game.State.Inventory.Deck;
using Game.State.Inventory.TowerCards;
using Game.State.Root;
using MVVM.CMD;

namespace Game.MainMenu.Commands.InventoryCommands
{
    public class CommandCreateInventoryHandler : ICommandHandler<CommandCreateInventory>
    {
        private readonly GameStateProxy _gameState;
        private readonly GameSettings _gameSettings;
        private readonly ICommandProcessor _cmd;

        public CommandCreateInventoryHandler(
            GameStateProxy gameState, 
            GameSettings gameSettings, 
            ICommandProcessor cmd)
        {
            _gameState = gameState;
            _gameSettings = gameSettings;
            _cmd = cmd;
        }
        
        public bool Handle(CommandCreateInventory commandCreate)
        {
            var towerCards = _gameSettings.InventoryInitialSettings.TowerCards;
            var towerPlans = _gameSettings.InventoryInitialSettings.TowerPlans;
            var configTowers = _gameSettings.TowersSettings.AllTowers;
            //Debug.Log("Загрузка инвентаря из настроек " + JsonConvert.SerializeObject(towers, Formatting.Indented));

            foreach (var towerPlan in towerPlans)
            {
                var commandTowerPlan = new CommandTowerPlanAdd
                {
                    ConfigId = towerPlan.ConfigId,
                    Amount = towerPlan.Amount
                };
                _cmd.Process(commandTowerPlan, false);
            }
            
            foreach (var towerCard in towerCards) //Начальные башни из настроек
            {
                var configTower = configTowers.FirstOrDefault(t => t.ConfigId == towerCard.ConfigId);
                if (configTower == null) throw new Exception($"towerConfig = {towerCard.ConfigId}  Not Find");
                var commandTowerCard = new CommandTowerCardAdd
                {
                    ConfigId = towerCard.ConfigId,
                    EpicLevel = towerCard.epicCardLevel,
                    Level = towerCard.Level
                };
                _cmd.Process(commandTowerCard, false);
            }
            
            var initialDeck = new DeckCardData(); //Создаем начальную колоду
            var index = 0;
            foreach (var inventoryItem in _gameState.Inventory.Items)
            {
                
                if (inventoryItem is TowerCard towerCard && index < 7)
                {
                    index++;
                    initialDeck.TowerCardIds.Add(index, towerCard.UniqueId); //Добавляем начальные башни в колоду    
                }
            }
            _gameState.Inventory.DeckCards.Add(1, new DeckCard(initialDeck));
            
            //TODO Начальные навыки из настроек
            //TODO Начальный герой из настроек
            
            
            _gameState.HardCurrency.OnNext(5000);
            _gameState.SoftCurrency.OnNext(45000);
            return true;
        }
    }
}