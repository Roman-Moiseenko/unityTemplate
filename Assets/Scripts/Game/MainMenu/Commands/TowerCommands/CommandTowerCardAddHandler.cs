using System;
using System.Collections.Generic;
using System.Linq;
using Game.Settings;
using Game.State.Inventory.TowerCards;
using Game.State.Maps.Towers;
using Game.State.Root;
using MVVM.CMD;

namespace Game.MainMenu.Commands.TowerCommands
{
    public class CommandTowerCardAddHandler : ICommandHandler<CommandTowerCardAdd>
    {
        private readonly GameStateProxy _gameState;
        private readonly GameSettings _gameSettings;

        public CommandTowerCardAddHandler(GameStateProxy gameState, GameSettings gameSettings)
        {
            _gameState = gameState;
            _gameSettings = gameSettings;
        }
        public bool Handle(CommandTowerCardAdd command)
        {
            var towers = _gameSettings.TowersSettings.AllTowers;
            var towerConfig = towers.FirstOrDefault(t => t.ConfigId == command.ConfigId);
            
            if (towerConfig == null) throw new Exception($"towerConfig = {command.ConfigId}  Not Find");

            
            var initialTowerCard = new TowerCardData
            {
                UniqueId = _gameState.CreateInventoryID(),
                ConfigId = command.ConfigId,
                EpicLevel = command.EpicLevel,
                Level = command.Level,
                Amount = 1, //towerCard.Amount,
                Parameters = new Dictionary<TowerParameterType, TowerParameterData>(),
            };
               
            foreach (var baseParameter in towerConfig.BaseParameters)
            {
                initialTowerCard.Parameters.Add(baseParameter.ParameterType, new TowerParameterData(baseParameter));
            }
                
            _gameState.Inventory.AddItem(initialTowerCard);
            return true;
        }
    }
}