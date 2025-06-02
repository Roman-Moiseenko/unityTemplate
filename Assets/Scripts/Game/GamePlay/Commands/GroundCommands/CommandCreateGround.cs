using Game.State.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.GroundCommands
{
    public class CommandCreateGround: ICommand
    {
        public readonly string GroundType;
        public readonly Vector2Int Position;

        public CommandCreateGround(string groundType, Vector2Int position)
        {
            GroundType = groundType;
            Position = position;
        }
    }
}