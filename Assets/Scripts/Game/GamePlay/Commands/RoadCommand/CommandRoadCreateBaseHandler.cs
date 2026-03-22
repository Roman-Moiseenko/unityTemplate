using System;
using System.Collections.Generic;
using System.Linq;
using Game.Settings.Gameplay.Entities.Road;
using Game.State.Gameplay;
using Game.State.Maps.Roads;
using Game.State.Root;
using MVVM.CMD;
using Newtonsoft.Json;
using UnityEngine;
using Random = System.Random;

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
         /*   var mainRoads = new List<RoadInitialSettings>
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
            */
            var mainRoads = GeneratePath(true, command.hasWaySecond).ToList();
           // if (command.hasWaySecond) GeneratePath(false, true).ToList();
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
                /*
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
                */
                var secondRoads = GeneratePath(false, true).ToList();
                
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


            
           // GeneratePath(true, true);
            
            return false;
        }

        private List<RoadInitialSettings> GeneratePath(bool first, bool hasSecond)
        {
            var posY = 0;
            if (first && hasSecond) posY = 1;
            if(!first) posY = -1;

            var random = new Random();
            var countRoads = random.Next(4, 6);
            var roads = new List<RoadInitialSettings>();

            var points = new List<Vector2Int>
            {
                new(0, posY),
                new(1, posY),
                new(2, posY),
            };
            
            for (int i = 2; i < countRoads; i++)
            {
                var outPoint = points[i - 1] - points[i];
                var delta = getNextPoint(outPoint, posY);
                points.Add(delta + points[i]);
            }

            for (int i = 1; i < points.Count - 1; i++)
            {
                roads.Add(getNextRoad(points[i + 1], points[i], points[i - 1]));
            }
            
            return roads;
        }

        private Vector2Int getNextPoint(Vector2Int point, int posY)
        {
            var list = new List<Vector2Int>();
            var v1 = new Vector2Int(1, 0);
            var v2 = new Vector2Int(0, -1);
            var v3 = new Vector2Int(0, 1);
            if (point != v1) list.Add(v1);
            if (point != v2 && posY != 1) list.Add(v2);
            if (point != v3 && posY != -1) list.Add(v3);
            var random = new Random();
            var index = random.Next(list.Count);
            return list[index];
        }

        private RoadInitialSettings getNextRoad(Vector2Int nextPoint, Vector2Int zeroPosition, Vector2Int previousPoint)
        {
            RoadInitialSettings road = new();
            road.Position = zeroPosition;
            if (nextPoint.x == previousPoint.x)
            {
                road.Rotate = 1;
                road.IsTurn = false;
                return road;
            }
            if (nextPoint.y == previousPoint.y)
            {
                road.Rotate = 2;
                road.IsTurn = false;
                return road;
            }
            road.IsTurn = true;
            var dist = nextPoint - previousPoint;

            if (dist == new Vector2Int(1, 1))
            {
                if (zeroPosition == previousPoint + new Vector2Int(1, 0))
                {
                    road.Rotate = 0;
                    return road;
                }
                if (zeroPosition == previousPoint + new Vector2Int(0, 1))
                {
                    road.Rotate = 2;
                    return road;
                }

                throw new Exception("Error");
            }
            if (dist == new Vector2Int(1, -1))
            {
                if (zeroPosition == previousPoint + new Vector2Int(1, 0))
                {
                    road.Rotate = 3;
                    return road;
                }
                if (zeroPosition == previousPoint + new Vector2Int(0, -1))
                {
                    road.Rotate = 1;
                    return road;
                }
                throw new Exception("Error");
            }
            throw new Exception("Error");
        }
    }
}