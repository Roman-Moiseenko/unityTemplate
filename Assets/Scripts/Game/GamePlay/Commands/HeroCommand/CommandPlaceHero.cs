using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.HeroCommand
{
    public class CommandPlaceHero  : ICommand
    {
        public Vector2Int Position;

        public CommandPlaceHero(Vector2Int position)
        {
            Position = position;
        }
    }
}