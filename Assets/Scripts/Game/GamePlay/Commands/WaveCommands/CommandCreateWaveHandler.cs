using System;
using System.Collections.Generic;
using System.Linq;
using Game.GameRoot.Services;
using Game.Settings;
using Game.Settings.Gameplay.Maps;
using Game.State.Maps.Mobs;
using Game.State.Maps.Waves;
using Game.State.Root;
using MVVM.CMD;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.GamePlay.Commands.WaveCommands
{
    public class CommandCreateWaveHandler : ICommandHandler<CommandCreateWave>
    {
        private readonly GameSettings _gameSettings;
        private readonly GameplayStateProxy _gameplayState;
        private readonly GenerateService _generateService;
        private readonly InfinitySetting _infinitySetting;

        public CommandCreateWaveHandler(GameSettings gameSettings, GameplayStateProxy gameplayState,
            GenerateService generateService)
        {
            _gameSettings = gameSettings;
            _gameplayState = gameplayState;
            _generateService = generateService;
            _infinitySetting = _gameSettings.MapsSettings.InfinitySetting;
        }

        public bool Handle(CommandCreateWave command)
        {
            var newMapSettings =
                _gameSettings.MapsSettings.Maps.First(m => m.MapId == _gameplayState.MapId.CurrentValue);
            var newMapInitialStateSettings = newMapSettings.InitialStateSettings;
            var waveItems = newMapInitialStateSettings.Waves[command.Index - 1].WaveItems;

            foreach (var waveItem in waveItems) //Настройки каждой волны - группы мобов
            {
                //Добавляем кол-во данного типа мобов в список
                for (int i = 0; i < waveItem.Quantity; i++)
                {
                    var mobConfig = _gameSettings.MobsSettings.AllMobs
                        .Find(m => m.ConfigId == waveItem.MobConfigId);
                    if (mobConfig == null)
                    {
                        //Ищем среди боссов
                        mobConfig = _gameSettings.MobsSettings.AllBosses
                            .Find(m => m.ConfigId == waveItem.MobConfigId);
                    }

                    if (mobConfig == null)
                        throw new Exception($"Не найдена конфигурация {waveItem.MobConfigId} моба.");
                    //TODO Переделать получения ratioCurveMobs Либо константа, либо из настроек каждого моба
                    var levelCoef = _generateService.GetRatioCurve(waveItem.Level, _infinitySetting.ratioCurveMobs);
                    var mob = new MobEntityData
                    {
                        ConfigId = mobConfig.ConfigId,
                        UniqueId = _gameplayState.CreateEntityID(),
                        Health = mobConfig.Health * levelCoef,
                        Armor = mobConfig.Armor,
                        Attack = mobConfig.Attack * levelCoef,
                        SpeedMove = mobConfig.SpeedMove,
                        SpeedAttack = mobConfig.SpeedAttack,
                        IsFly = mobConfig.IsFly,
                        RewardCurrency = (int)(mobConfig.RewardCurrency * levelCoef),
                        Level = waveItem.Level,
                        Defence = mobConfig.Defence,
                        NumberWave = command.Index
                    };
                    _gameplayState.Mobs.Add(new MobEntity(mob));
                }
            }

            return false; //Волну в не сохраняем, сохранение идет в создании уровня
        }
    }
}