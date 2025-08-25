using System;
using System.Collections.Generic;
using System.Linq;
using Game.Settings;
using Game.State.Inventory;
using Game.State.Inventory.Deck;
using Game.State.Inventory.TowerCards;
using Game.State.Maps.Towers;
using Game.State.Root;
using JetBrains.Annotations;
using MVVM.CMD;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.GamePlay.Commands.Inventory
{
    public class CommandCreateInventoryHandler : ICommandHandler<CommandCreateInventory>
    {
        private readonly GameStateProxy _gameState;
        private readonly GameSettings _gameSettings;

        public CommandCreateInventoryHandler(GameStateProxy gameState, GameSettings gameSettings)
        {
            _gameState = gameState;
            _gameSettings = gameSettings;
        }
        
        public bool Handle(CommandCreateInventory commandCreate)
        {
           
            var towerCards = _gameSettings.InventoryInitialSettings.TowerCards;
            var towers = _gameSettings.TowersSettings.AllTowers;
            Debug.Log("Загрузка инвентаря из настроек " + JsonConvert.SerializeObject(towers, Formatting.Indented));
            var initialDeck = new DeckCardData(); //Создаем начальную колоду
             
                            
            var index = 0;
            foreach (var towerCard in towerCards) //Начальные башни из настроек
            {
                var towerConfig = towers.FirstOrDefault(t => t.ConfigId == towerCard.ConfigId);
                if (towerConfig == null) throw new Exception($"towerConfig = {towerCard.ConfigId}  Not Find");
                
                index++;
                //TODO Команда
                 
                var initialTowerCard = new TowerCardData
                {
                    UniqueId = _gameState.CreateInventoryID(),
                  //  TypeItem = InventoryType.TowerCard,
                    ConfigId = towerCard.ConfigId,
                    EpicLevel = towerCard.epicCardLevel,
                    Level = towerCard.Level,
                    Amount = 1, //towerCard.Amount,
                    Parameters = new Dictionary<TowerParameterType, TowerParameterData>(),
                };
               
                foreach (var baseParameter in towerConfig.BaseParameters)
                {
                    initialTowerCard.Parameters.Add(baseParameter.ParameterType, new TowerParameterData(baseParameter));
                }
                
                _gameState.Inventory.AddItem(initialTowerCard);
                
                initialDeck.TowerCardIds.Add(index, initialTowerCard.UniqueId); //Добавляем начальные башни в колоду
            }
            //TODO Начальные навыки из настроек
            //TODO Начальный герой из настроек
            
//            Debug.Log("5555 " + JsonConvert.SerializeObject(initialDeck, Formatting.Indented));
            _gameState.Inventory.DeckCards.Add(1, new DeckCard(initialDeck));
            _gameState.HardCurrency.Value = 5000;
            return true;
        }
    }
}