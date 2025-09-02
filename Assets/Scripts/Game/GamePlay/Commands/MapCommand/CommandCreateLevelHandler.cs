using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.Classes;
using Game.GamePlay.Commands.CastleCommands;
using Game.GamePlay.Commands.GroundCommands;
using Game.GamePlay.Commands.RoadCommand;
using Game.GamePlay.Commands.TowerCommand;
using Game.GamePlay.Commands.WaveCommands;
using Game.Settings;
using Game.State.Maps.Castle;
using Game.State.Maps.Roads;
using Game.State.Root;
using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.MapCommand
{
    public class CommandCreateLevelHandler
        : ICommandHandler<CommandCreateLevel>
    {
        private readonly GameSettings _gameSettings;
        private readonly GameplayStateProxy _gameplayState;
        private readonly ICommandProcessor _cmd;

        public CommandCreateLevelHandler(GameSettings gameSettings, GameplayStateProxy gameplayState,
            ICommandProcessor cmd)
        {
            _gameSettings = gameSettings;
            _gameplayState = gameplayState;
            _cmd = cmd;
        }

        public bool Handle(CommandCreateLevel command)
        {
            if (_gameplayState.Towers.Any())
            {
                Debug.Log($"Map id={command.MapId} already exist");
                return false;
            }
            //Находим настройки карты по ее Id
            var newMapSettings = _gameSettings.MapsSettings.Maps.First(m => m.MapId == command.MapId);
            var newMapInitialStateSettings = newMapSettings.InitialStateSettings;

            //Генерируем поверхность
            var commandGround = new CommandGroundCreateBase
            {
                IsSmall = newMapInitialStateSettings.smallMap,
                GroundConfigId = newMapInitialStateSettings.groundDefault,
                Collapse = newMapInitialStateSettings.collapse,
                Obstacle = newMapInitialStateSettings.obstacle
            };
            _cmd.Process(commandGround);

            //Генерируем дороги
            var commandRoads = new CommandRoadCreateBase
            {
                RoadConfigId = newMapInitialStateSettings.roadDefault,
                hasWaySecond = newMapInitialStateSettings.hasWaySecond,
                hasWayDisabled = newMapInitialStateSettings.hasWayDisabled
            };
            _cmd.Process(commandRoads);
            
            //Добавляем Волны мобов
            for (var index = 0; index < newMapInitialStateSettings.Waves.Count; index++)
            {
                var commandWave = new CommandCreateWave
                {
                    Index = index + 1,
                    WaveItems = newMapInitialStateSettings.Waves[index].WaveItems
                };
                _cmd.Process(commandWave);
            }
            
            //Размещаем крепость
            var commandCastle = new CommandCastleCreate();
            _cmd.Process(commandCastle);


            //Размещаем базовые башни, если имеются
            foreach (var towerSettings in newMapInitialStateSettings.Towers)
            {
                var commandTower = new CommandPlaceTower(towerSettings.ConfigId, towerSettings.Position);
                _cmd.Process(commandTower);
            }

            _gameplayState.CurrentWave.Value = 0;
            _gameplayState.MapId.Value = command.MapId;
            _gameplayState.SetTypeGameplay(TypeGameplay.Levels);
            return true;
        }
    }
}