using Game.State.Gameplay;
using MVVM.CMD;

namespace Game.GamePlay.Commands.HeroCommand
{
    public class CommandPlaceHeroHandler : ICommandHandler<CommandPlaceHero>
    {
        private readonly GameplayStateProxy _gameplayState;

        public CommandPlaceHeroHandler(GameplayStateProxy gameplayState)
        {
            _gameplayState = gameplayState;
        }
        public bool Handle(CommandPlaceHero command)
        {
            _gameplayState.Hero.Position.Value = command.Position;
            return  true;
        }
    }
}