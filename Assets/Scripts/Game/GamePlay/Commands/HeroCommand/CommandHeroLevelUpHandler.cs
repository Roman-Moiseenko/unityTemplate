using Game.State.Gameplay;
using MVVM.CMD;

namespace Game.GamePlay.Commands.HeroCommand
{
    public class CommandHeroLevelUpHandler : ICommandHandler<CommandHeroLevelUp>
    {
        private readonly GameplayStateProxy _gameplayState;

        public CommandHeroLevelUpHandler(GameplayStateProxy gameplayState)
        {
            _gameplayState = gameplayState;
        }
        public bool Handle(CommandHeroLevelUp command)
        {
            _gameplayState.Hero.GameplayLevel.Value++;
            return  true;
        }
    }
}