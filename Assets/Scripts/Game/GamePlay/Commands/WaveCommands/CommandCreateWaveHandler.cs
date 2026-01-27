using System.Linq;
using Game.GamePlay.Commands.MobCommands;
using Game.Settings;
using Game.State.Root;
using MVVM.CMD;

namespace Game.GamePlay.Commands.WaveCommands
{
    /**
     * Создание волны мобов. Получаем настройки волны (кол-во, тип и уровень мобов)
     * и запускаем команду создания мобов по каждому типу и уровню
     */
    public class CommandCreateWaveHandler : ICommandHandler<CommandCreateWave>
    {
        private readonly GameSettings _gameSettings;
        private readonly GameplayStateProxy _gameplayState;
        private readonly ICommandProcessor _cmd;
        
        public CommandCreateWaveHandler(GameSettings gameSettings, 
            GameplayStateProxy gameplayState, ICommandProcessor cmd)
        {
            _gameSettings = gameSettings;
            _gameplayState = gameplayState;
            _cmd = cmd;
        }

        public bool Handle(CommandCreateWave command)
        {
            var newMapSettings =
                _gameSettings.MapsSettings.Maps.First(m => m.MapId == _gameplayState.MapId.CurrentValue);
            var newMapInitialStateSettings = newMapSettings.InitialStateSettings;
            var waveItems = newMapInitialStateSettings.Waves[command.Index - 1].WaveItems;

            foreach (var waveItem in waveItems) //Настройки каждой волны - группы мобов
            {
                var commandMob = new CommandCreateMob
                {
                    ConfigId = waveItem.MobConfigId,
                    Level = waveItem.Level,
                    NumberWave = command.Index,
                    Quantity = waveItem.Quantity,
                };
                _cmd.Process(commandMob);
            }
            return false; //Волну в не сохраняем
        }
    }
}