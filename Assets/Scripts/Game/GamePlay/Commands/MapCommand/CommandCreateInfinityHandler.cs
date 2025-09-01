using System.Collections.Generic;
using Game.GamePlay.Commands.CastleCommands;
using Game.GamePlay.Commands.GroundCommands;
using Game.GamePlay.Commands.RoadCommand;
using Game.GamePlay.Commands.WaveCommands;
using Game.GamePlay.Services;
using Game.Settings;
using Game.Settings.Gameplay.Enemies;
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

        private const float Coef = 1.021f;
        private const int MaxWave = 1000;

        private const int BaseWaveHealth = 760;
        public CommandCreateInfinityHandler(GameSettings gameSettings, GameplayStateProxy gameplayState, ICommandProcessor cmd)
        {
            _gameSettings = gameSettings;
            _gameplayState = gameplayState;
            _cmd = cmd;
        }
        public bool Handle(CommandCreateInfinity command)
        {
            //Генерируем поверхность
            var newMapSettings = _gameSettings.MapsSettings;
            var groundConfigIds = newMapSettings.GroundConfigIds;
            var index = (int)Mathf.Ceil(Mathf.Abs(Random.insideUnitSphere.x) / groundConfigIds.Count);
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
            index = (int)Mathf.Ceil(Mathf.Abs(Random.insideUnitSphere.x) / roadConfigIds.Count);
            var commandRoads = new CommandRoadCreateBase
            {
                RoadConfigId = roadConfigIds[index],
                hasWaySecond = false,
                hasWayDisabled = false
            };
            _cmd.Process(commandRoads);
            
           
            //Добавляем Волны мобов
            for (var i = 0; index < MaxWave; index++)
            {
                var commandWave = new CommandCreateWave
                {
                    Index = i + 1,
                    WaveItems = GenerateWave(i),
                };
                _cmd.Process(commandWave);
            }
            
            //Размещаем крепость
            var commandCastle = new CommandCastleCreate();
            _cmd.Process(commandCastle);
            _gameplayState.CurrentWave.Value = 0;
            _gameplayState.MapId.Value = command.UniqueId;
            return true;
        }

        private List<WaveItemSettings> GenerateWave(int numberWave)
        {
            //TODO Генерация волн
            const float coefLevelMobFromWave = 3f;
            
            var list = new List<WaveItemSettings>();
            
            var waveHealth = GenerateService.GetWaveHealth(numberWave);
            var levelMob = Mathf.Ceil(numberWave / coefLevelMobFromWave);
            
            Debug.Log($"wave = {numberWave} Health = {waveHealth}");

            if (numberWave % 10 == 0)
            {
                
                //Уровень босса numberWave / 10
                //Random 2й босс, после 20 волны и тип босса разный
                
                //После 100 волны добавляем мобов 
                var bcw = numberWave / 100;

            }
            else
            {
                
                
                
            }
            
            throw new System.NotImplementedException();
        }
        
    }
}