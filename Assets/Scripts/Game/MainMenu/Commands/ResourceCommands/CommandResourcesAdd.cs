using Game.State.CMD;
using Game.State.GameResources;

namespace Game.MainMenu.Commands.ResourceCommands
{
    public class CommandResourcesAdd : ICommand
    {
        public readonly ResourceType ResourceType;
        public readonly int Amount;

        public CommandResourcesAdd(ResourceType resourceType, int amount)
        {
            ResourceType = resourceType;
            Amount = amount;
        }
    }
}