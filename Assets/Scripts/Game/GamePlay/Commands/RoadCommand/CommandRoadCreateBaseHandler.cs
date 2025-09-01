using System.Collections.Generic;
using Game.Settings.Gameplay.Entities.Road;
using Game.State.Maps.Roads;
using Game.State.Root;
using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.RoadCommand
{
    public class CommandRoadCreateBaseHandler : ICommandHandler<CommandRoadCreateBase>
    {
        private readonly GameplayStateProxy _gameplayState;
        private readonly ICommandProcessor _cmd;

        public CommandRoadCreateBaseHandler(GameplayStateProxy gameplayState, ICommandProcessor cmd)
        {
            _gameplayState = gameplayState;
            _cmd = cmd;
        }
        public bool Handle(CommandRoadCreateBase command)
        {
            var mainY = command.hasWaySecond ? 1 : 0;
            var mainRoads = new List<RoadInitialSettings>
            {
                new()
                {
                    Position = new Vector2Int(1, mainY),
                    Rotate = 0,
                    IsTurn = false,
                },
                new()
                {
                    Position = new Vector2Int(2, mainY),
                    Rotate = 0,
                    IsTurn = false,
                },
                new()
                {
                    Position = new Vector2Int(3, mainY),
                    Rotate = 0,
                    IsTurn = true,
                }
            };

            //Рисуем основную дорогу
            foreach (var road in mainRoads)
            {
                var commandRoad = new CommandPlaceRoad(
                    command.RoadConfigId,
                    road.Position, road.IsTurn, road.Rotate, true);
                _cmd.Process(commandRoad);
            }

            if (command.hasWaySecond)
            {
                var secondRoads = new List<RoadInitialSettings>
                {
                    new()
                    {
                        Position = new Vector2Int(1, -1),
                        Rotate = 0,
                        IsTurn = false,
                    },
                    new()
                    {
                        Position = new Vector2Int(2, -1),
                        Rotate = 0,
                        IsTurn = false,
                    },
                    new()
                    {
                        Position = new Vector2Int(3, -1),
                        Rotate = 3,
                        IsTurn = true,
                    },
                    new()
                    {
                        Position = new Vector2Int(3, -2),
                        Rotate = 1,
                        IsTurn = false,
                    }
                };
                
                
                
                foreach (var road in secondRoads)
                {
                    var commandRoad = new CommandPlaceRoad(
                        command.RoadConfigId,
                        road.Position, road.IsTurn, road.Rotate, false);
                    _cmd.Process(commandRoad);
                }
            }

            if (command.hasWayDisabled)
            {
                /*
                foreach (var road in newMapInitialStateSettings.WayDisabled)
                {
                    var initialRoad = new RoadEntityData
                    {
                        UniqueId = _gameplayState.CreateEntityID(),
                        Position = road.Position,
                        ConfigId = command.RoadConfigId,
                        Rotate = road.Rotate,
                        IsTurn = road.IsTurn,
                    };
                    _gameplayState.WayDisabled.Add(
                        new RoadEntity(initialRoad)); // Entities.Add(EntitiesFactory.CreateEntity(initialRoad));
                }
                */
            }

            return false;
        }
    }
}