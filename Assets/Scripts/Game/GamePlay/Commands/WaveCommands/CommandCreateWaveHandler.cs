using System;
using System.Collections.Generic;
using Game.GamePlay.Services;
using Game.Settings;
using Game.State.Maps.Mobs;
using Game.State.Maps.Waves;
using Game.State.Root;
using MVVM.CMD;

namespace Game.GamePlay.Commands.WaveCommands
{
    public class CommandCreateWaveHandler  : ICommandHandler<CommandCreateWave>
    {
        private readonly GameSettings _gameSettings;
        private readonly GameplayStateProxy _gameplayState;

      //  private readonly GenerateService _generateService;
        // private readonly ICommandProcessor _cmd;

        public CommandCreateWaveHandler(GameSettings gameSettings, GameplayStateProxy gameplayState)
        {
            _gameSettings = gameSettings;
            _gameplayState = gameplayState;
            
            //      _cmd = cmd;
        }
        public bool Handle(CommandCreateWave command)
        {
            var initialWave = new WaveEntityData //Создаем волну и присваиваем ей порядковый номер
            {
                Number = command.Index,
                Mobs = new List<MobEntityData>(),
            };
            
            foreach (var waveItem in command.WaveItems) //Настройки каждой волны - группы мобов
            {
                //Коэфициент усиления от уровня
                
                //Добавляем кол-во данного типа мобов в список
                for (int i = 0; i < waveItem.Quantity; i++)
                {
                    var mobConfig = _gameSettings.MobsSettings.AllMobs
                        .Find(m => m.ConfigId == waveItem.MobConfigId);
                    if (mobConfig == null)
                        throw new Exception($"Не найдена конфигурация {waveItem.MobConfigId} моба.");
                    
                    var mobParameters = mobConfig.Parameters
                        .Find(p => p.Level == waveItem.Level);
                    if (mobParameters == null)
                        throw new Exception($"Не найден уровень {waveItem.Level} в конфигурации {waveItem.MobConfigId} моба.");
                    var p = GenerateService.GenerateMobParameters(mobConfig.BaseParameters, waveItem.Level);
                    
                    var mob = new MobEntityData
                    {
                        ConfigId = mobConfig.ConfigId,
                        UniqueId = _gameplayState.CreateEntityID(),
                        Health = mobParameters.Health,
                        Type = mobConfig.Type,
                        Armor = mobParameters.Armor,
                        Attack = mobParameters.Attack,
                        Speed = mobConfig.Speed,
                        IsFly = mobConfig.IsFly,
                        RewardCurrency = mobParameters.RewardCurrency,
                        Level = mobParameters.Level,
                        Defence = mobConfig.Defence,
                    };
                    initialWave.Mobs.Add(mob);
                }
            }
            _gameplayState.Waves.Add(command.Index, new WaveEntity(initialWave));
            return false; //Волну в не сохраняем, сохранение идет в создании уровня
        }
    }
}