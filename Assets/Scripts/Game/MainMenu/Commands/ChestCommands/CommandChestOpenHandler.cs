using Game.MainMenu.Commands.InventoryCommands;
using Game.State.Root;
using MVVM.CMD;

namespace Game.MainMenu.Commands.ChestCommands
{
    public class CommandChestOpenHandler : ICommandHandler<CommandChestOpen>
    {
        private GameStateProxy _gameState;
        private readonly ICommandProcessor _cmd;

        public CommandChestOpenHandler(GameStateProxy gameState, ICommandProcessor cmd)
        {
            _gameState = gameState;
            _cmd = cmd;
        }
        public bool Handle(CommandChestOpen command)
        {
            var chest = _gameState.ContainerChests.Chests[command.Cell];
            //Сохраняем награды
            foreach (var reward in command.Rewards)
            {
                
                    var commandReward = new CommandInventoryFromReward()
                    {
                        InventoryType = reward.RewardType,
                        ConfigId = reward.ConfigId,
                        Amount = reward.Amount
                    };
                    _cmd.Process(commandReward);
                
            }

            //Сундук удаляем
            _gameState.ContainerChests.Chests.Remove(chest.Cell);
            return true;
        }
    }
}