using MVVM.CMD;

namespace Game.GameRoot.Commands
{
    public class CommandSaveGameStateHandler : ICommandHandler<CommandSaveGameState>
    {
        public bool Handle(CommandSaveGameState command)
        {
            return true;
        }
    }
}