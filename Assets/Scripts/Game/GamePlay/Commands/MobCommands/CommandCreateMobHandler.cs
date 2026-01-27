using System;
using System.Collections.Generic;
using Game.Settings;
using Game.State.Maps.Mobs;
using Game.State.Root;
using MVVM.CMD;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.GamePlay.Commands.MobCommands
{
    /**
     * Создание мобов из настроек
     */
    public class CommandCreateMobHandler : ICommandHandler<CommandCreateMob>
    {
        private readonly GameSettings _gameSettings;
        private readonly GameplayStateProxy _gameplayState;

        public CommandCreateMobHandler(GameSettings gameSettings, GameplayStateProxy gameplayState)
        {
            _gameSettings = gameSettings;
            _gameplayState = gameplayState;
        }

        public bool Handle(CommandCreateMob command)
        {
            var mobSettings = _gameSettings.MobsSettings.AllMobs
                .Find(m => m.ConfigId == command.ConfigId);
            //Ищем среди боссов
            if (mobSettings == null)
                mobSettings = _gameSettings.MobsSettings.AllBosses
                    .Find(m => m.ConfigId == command.ConfigId);

            if (mobSettings == null) throw new Exception($"Не найдена конфигурация {command.ConfigId} моба.");
            
            var healthOfLevel = mobSettings.Parameters[MobParameter.Health];
            var damageOfLevel = mobSettings.Parameters[MobParameter.Damage];

            //TODO Получить из настроек награды и другое 
            MobParameter? damageSecondType = null;
            float damageSecond = 0;
            foreach (var (type, list) in mobSettings.Parameters)
            {
                if (type.IsDamage())
                {
                    damageSecondType = type;
                    damageSecond = list[command.Level - 1];
                }
            }
            
            for (var i = 0; i < command.Quantity; i++)
            {
                var mob = new MobEntityData
                {
                    ConfigId = mobSettings.ConfigId,
                    UniqueId = _gameplayState.CreateEntityID(),
                    Health = healthOfLevel[command.Level - 1], //mobSettings.Health * levelCoef,
                    Damage = damageOfLevel[command.Level - 1], //mobSettings.Attack * levelCoef,
                    SpeedMove = mobSettings.SpeedMove,
                    SpeedAttack = mobSettings.SpeedAttack,
                    IsFly = mobSettings.IsFly,
                    RewardCurrency = (int)(mobSettings.RewardCurrency * command.Level),
                    Level = command.Level,
                    Defence = mobSettings.Defence,
                    NumberWave = command.NumberWave,
                    DamageSecond = damageSecond,
                    DamageSecondType = damageSecondType,
                };
                
                _gameplayState.Mobs.Add(new MobEntity(mob));
            }

            return false;
        }
    }
}