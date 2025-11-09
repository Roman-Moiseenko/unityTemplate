using Game.GamePlay.Classes;
using Game.GamePlay.Commands.CastleCommands;
using Game.GamePlay.Commands.GroundCommands;
using Game.GamePlay.Commands.RoadCommand;
using Game.GamePlay.Commands.WaveCommands;
using Game.Settings;
using Game.State.Root;
using MVVM.CMD;

namespace Game.GamePlay.Commands.MapCommand
{
    public class CommandCreateInfinityHandler : ICommandHandler<CommandCreateInfinity>
    {
        private readonly GameSettings _gameSettings;
        private readonly GameplayStateProxy _gameplayState;
        private readonly ICommandProcessor _cmd;
        private readonly string _defaultGroundConfigId;
        private readonly string _defaultRoadConfigId;

        public CommandCreateInfinityHandler(GameSettings gameSettings,
            GameplayStateProxy gameplayState, ICommandProcessor cmd, 
            string defaultGroundConfigId, 
            string defaultRoadConfigId
        )
        {
            _gameSettings = gameSettings;
            _gameplayState = gameplayState;
            _cmd = cmd;
            _defaultGroundConfigId = defaultGroundConfigId;
            _defaultRoadConfigId = defaultRoadConfigId;
        }

        public bool Handle(CommandCreateInfinity command)
        {
            //Генерируем поверхность

            var commandGround = new CommandGroundCreateBase
            {
                IsSmall = false,
                GroundConfigId = _defaultGroundConfigId,
                Collapse = 0,
                Obstacle = false
            };
            _cmd.Process(commandGround, false);

            //Генерируем дороги

            var commandRoads = new CommandRoadCreateBase
            {
                RoadConfigId = _defaultRoadConfigId,
                hasWaySecond = false,
                hasWayDisabled = false
            };
            _cmd.Process(commandRoads, false);
            
            //Добавляем Волны мобов
            for (var i = 0; i < 10; i++)
            {
                var commandWave = new CommandWaveGenerate(i + 1);
                _cmd.Process(commandWave, false);
            }

            //Размещаем крепость
            var commandCastle = new CommandCastleCreate();
            _cmd.Process(commandCastle, false);
            _gameplayState.CurrentWave.Value = 0;
            _gameplayState.MapId.OnNext(command.UniqueId);
            _gameplayState.SetTypeGameplay(TypeGameplay.Infinity);
            return true;
        }
    }
}