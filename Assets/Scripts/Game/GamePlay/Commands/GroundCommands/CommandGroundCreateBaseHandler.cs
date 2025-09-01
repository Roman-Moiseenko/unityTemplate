using System.Collections.Generic;
using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.GroundCommands
{
    public class CommandGroundCreateBaseHandler : ICommandHandler<CommandGroundCreateBase>
    {
        private readonly ICommandProcessor _cmd;

        public CommandGroundCreateBaseHandler(ICommandProcessor cmd)
        {
            _cmd = cmd;
        }
        public bool Handle(CommandGroundCreateBase command)
        {
            
            //TODO добавить препятствия и провалы на карту command.Collapse; command.Obstacle;
            var yDelta = command.IsSmall ? 3 : 4;


            var xBegin = -1;
            var xEnd = command.IsSmall ? 5 : 6;
            var yBegin = 1 - yDelta;
            var yEnd = yDelta;

            var angel0 = new Vector2Int(xBegin, yBegin);
            var angel1 = new Vector2Int(xBegin, yEnd);
            var angel2 = new Vector2Int(xEnd, yBegin);
            var angel3 = new Vector2Int(xEnd, yEnd);

            var listExceptions = new List<Vector2Int>
            {
                angel0,
                angel1,
                angel2,
                angel3,
                angel0 + Vector2Int.up,
                angel0 + Vector2Int.right,
                angel1 + Vector2Int.down,
                angel1 + Vector2Int.right,
                angel2 + Vector2Int.up,
                angel2 + Vector2Int.left,
                angel3 + Vector2Int.down,
                angel3 + Vector2Int.left
            };

            for (var x = xBegin; x <= xEnd; x++)
            {
                for (var y = yBegin; y <= yEnd; y++)
                {
                    var position = new Vector2Int(x, y);
                    if (listExceptions.Contains(position)) continue;

                    var commandGround = new CommandCreateGround(command.GroundConfigId, position);
                    _cmd.Process(commandGround);
                }
            }

            return false;
        }
    }
}