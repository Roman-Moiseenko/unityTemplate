using System;
using System.Linq;
using Game.State.Inventory.TowerCards;
using Game.State.Root;
using MVVM.CMD;
using UnityEngine;

namespace Game.MainMenu.Commands.TowerCommands
{
    public class CommandTowerCardLevelUpHandler : ICommandHandler<CommandTowerCardLevelUp>
    {
        private readonly GameStateProxy _gameState;

        public CommandTowerCardLevelUpHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }
        public bool Handle(CommandTowerCardLevelUp command)
        {
            
            var itemEntity = _gameState.Inventory.Items
                .FirstOrDefault(item => item.UniqueId == command.UniqueId);

            if (itemEntity == null)
            {
                throw new Exception("ItemEntity not Founded!");
            }

            var towerCardEntity = itemEntity.As<TowerCard>();
            towerCardEntity.Level.Value++;
            
            return true;
        }
    }
}