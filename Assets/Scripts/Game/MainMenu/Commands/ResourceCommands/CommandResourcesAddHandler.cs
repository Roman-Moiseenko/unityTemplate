using System;
using System.Linq;
using Game.State.GameResources;
using Game.State.Root;
using MVVM.CMD;

namespace Game.MainMenu.Commands.ResourceCommands
{
    public class CommandResourcesAddHandler : ICommandHandler<CommandResourcesAdd>
    {
        private readonly GameStateProxy _gameState;

        public CommandResourcesAddHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }


        public bool Handle(CommandResourcesAdd command)
        {
            if (command.Amount < 0) throw new Exception("Amount < 0");
            
            var requiredResourceType = command.ResourceType;
            var requiredResource = _gameState.Resources.FirstOrDefault(resource => resource.ResourceType == requiredResourceType);
            if (requiredResource == null)
            {
                requiredResource = CreateNewResource(requiredResourceType);
            }

            requiredResource.Amount.Value += command.Amount;
            return true;
        }

        private Resource CreateNewResource(ResourceType resourceType)
        {
            var newResourceData = new ResourceData
            {
                ResourceType = resourceType,
                Amount = 0
            };
            var newResource = new Resource(newResourceData);
            _gameState.Resources.Add(newResource);
            return newResource;
        }
    }
}