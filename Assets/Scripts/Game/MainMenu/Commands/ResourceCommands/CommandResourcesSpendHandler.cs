using System;
using System.Linq;
using Game.State.CMD;
using Game.State.Root;
using UnityEngine;

namespace Game.MainMenu.Commands.ResourceCommands
{
    public class CommandResourcesSpendHandler : ICommandHandler<CommandResourcesSpend>
    {
        private readonly GameStateProxy _gameState;

        public CommandResourcesSpendHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }

        public bool Handle(CommandResourcesSpend command)
        {
            if (command.Amount < 0) throw new Exception("Amount < 0");

            var requiredResourceType = command.ResourceType;
            var requiredResource =
                _gameState.Resources.FirstOrDefault(resource => resource.ResourceType == requiredResourceType);
            if (requiredResource == null)
            {
                Debug.Log($"Resource {command.ResourceType} not created");
                return false;
            }

            if (requiredResource.Amount.Value < command.Amount) return false;
            requiredResource.Amount.Value -= command.Amount;

            return true;
        }
    }
}