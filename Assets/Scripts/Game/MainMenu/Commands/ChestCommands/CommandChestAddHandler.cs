using Game.State.Inventory.Chests;
using Game.State.Root;
using MVVM.CMD;

namespace Game.MainMenu.Commands.ChestCommands
{
    public class CommandChestAddHandler : ICommandHandler<CommandChestAdd>
    {
        private GameStateProxy _gameState;
        public CommandChestAddHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }
        public bool Handle(CommandChestAdd command)
        {
            for (var i = 1; i <= ContainerChests.MaxChest; i++)
            {
                if (_gameState.ContainerChests.Chests.TryGetValue(i, out var value)) continue;
                
                var chest = new ChestEntityData
                {
                    Level = command.LevelChest,
                    TypeChest = command.TypeChest,
                    Gameplay = command.Gameplay,
                    Status = StatusChest.Close,
                    Cell = i,
                    Wave = command.Wave,
                    MapId = command.MapId,
                };
                _gameState.ContainerChests.Chests.Add(i, new Chest(chest));
                return true;
            }

            return false;
        }
    }
}