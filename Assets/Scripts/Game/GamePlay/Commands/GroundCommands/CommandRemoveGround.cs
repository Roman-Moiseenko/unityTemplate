using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.GroundCommands
{
    public class CommandRemoveGround: ICommand
    {
        public readonly Vector2Int Position;

        public CommandRemoveGround(Vector2Int position)
        {
            Position = position;
        }
    }
}