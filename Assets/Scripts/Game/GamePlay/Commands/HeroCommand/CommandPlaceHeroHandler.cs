using Game.State.Gameplay;
using MVVM.CMD;
using UnityEngine;

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
            _gameplayState.Hero.Placement.Value = command.Position;
            return  true;
        }
    }
}