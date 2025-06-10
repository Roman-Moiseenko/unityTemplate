using Game.State.GameResources;
using MVVM.CMD;

namespace Game.MainMenu.Commands.ResourceCommands
{
    public class CommandResourcesSpend : ICommand
    {
        public readonly ResourceType ResourceType;
        public readonly int Amount;

        public CommandResourcesSpend(ResourceType resourceType, int amount)
        {
            ResourceType = resourceType;
            Amount = amount;
        }
    }
}