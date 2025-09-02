using Game.GamePlay.Classes;
using Game.GamePlay.Commands.CastleCommands;
using Game.GamePlay.Commands.GroundCommands;
using Game.GamePlay.Commands.RoadCommand;
using Game.GamePlay.Commands.WaveCommands;
using Game.Settings;
using Game.State.Root;
using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.MapCommand
{
    public class CommandCreateInfinityHandler : ICommandHandler<CommandCreateInfinity>
    {
        private readonly GameSettings _gameSettings;
        private readonly GameplayStateProxy _gameplayState;
        private readonly ICommandProcessor _cmd;

        public CommandCreateInfinityHandler(GameSettings gameSettings,
            GameplayStateProxy gameplayState, ICommandProcessor cmd
        )
        {
            _gameSettings = gameSettings;
            _gameplayState = gameplayState;
            _cmd = cmd;
        }

        public bool Handle(CommandCreateInfinity command)
        {
            Debug.Log("CommandCreateInfinityHandler");
            //Генерируем поверхность
            var newMapSettings = _gameSettings.MapsSettings;
            var groundConfigIds = newMapSettings.GroundConfigIds;
            var index = Mathf.Min(
                (int)Mathf.Ceil(Mathf.Abs(Random.insideUnitSphere.x) * groundConfigIds.Count),
                groundConfigIds.Count - 1
            );
            var commandGround = new CommandGroundCreateBase
            {
                IsSmall = false,
                GroundConfigId = groundConfigIds[index],
                Collapse = 0,
                Obstacle = false
            };
            _cmd.Process(commandGround);

            //Генерируем дороги
            var roadConfigIds = newMapSettings.RoadConfigIds;
            index = Mathf.Min(
                (int)Mathf.Ceil(Mathf.Abs(Random.insideUnitSphere.x) * roadConfigIds.Count),
                roadConfigIds.Count - 1
            );
            var commandRoads = new CommandRoadCreateBase
            {
                RoadConfigId = roadConfigIds[index],
                hasWaySecond = false,
                hasWayDisabled = false
            };
            _cmd.Process(commandRoads);
            
            //Добавляем Волны мобов
            for (var i = 0; i < 10; i++)
            {
                var commandWave = new CommandWaveGenerate(i + 1);
                _cmd.Process(commandWave);
            }

            //Размещаем крепость
            var commandCastle = new CommandCastleCreate();
            _cmd.Process(commandCastle);
            _gameplayState.CurrentWave.Value = 0;
            _gameplayState.MapId.OnNext(command.UniqueId);
            _gameplayState.SetTypeGameplay(TypeGameplay.Infinity);
            return true;
        }
    }
}