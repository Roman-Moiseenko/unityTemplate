using Game.State.CMD;
using Game.State.GameResources;

namespace Game.GamePlay.Commands
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