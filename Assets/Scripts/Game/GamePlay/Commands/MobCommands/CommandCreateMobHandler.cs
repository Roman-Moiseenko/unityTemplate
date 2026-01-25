using Game.State.Root;
using MVVM.CMD;

namespace Game.GamePlay.Commands.MobCommands
{
    public class CommandCreateMobHandler : ICommandHandler<CommandCreateMob>
    {
        private readonly GameplayStateProxy _gameplayState;

        public CommandCreateMobHandler(GameplayStateProxy gameplayState)
        {
            _gameplayState = gameplayState;
        }
        public bool Handle(CommandCreateMob command)
        {

            return false;
        }
    }
}