using System;
using System.Linq;
using DI;
using Game.Common;
using Game.GamePlay.Commands;
using Game.GamePlay.Commands.MapCommand;
using Game.GamePlay.Commands.RewardCommand;
using Game.GamePlay.Commands.TowerCommand;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.GamePlay.Services;
using Game.MainMenu.Services;
using Game.Settings;
using Game.State;
using MVVM.CMD;
using MVVM.FSM;
using Newtonsoft.Json;
using R3;
using Scripts.Game.GameRoot.Services;
using UnityEngine;

namespace Game.GamePlay.Root
{
    public static class GameplayRegistrations
    {
        /**
         * Регистрируем все сервисы для сцены, вручную
         */
        public static void Register(DIContainer container, GameplayEnterParams gameplayEnterParams)
        {
            var gameStateProvider = container.Resolve<IGameStateProvider>(); //Получаем репозиторий
            var gameState = gameStateProvider.GameState; //TODO Получим кристалы для изменения

            var gameplayState = gameStateProvider.GameplayState;
            var settingsProvider = container.Resolve<ISettingsProvider>();
            var gameSettings = settingsProvider.GameSettings;
            //Регистрируем машину состояния
            var Fsm = new FsmGameplay(container);
            container.RegisterInstance(Fsm);
            var subjectExitParams = new Subject<GameplayExitParams>();
            container.RegisterInstance(subjectExitParams); //Событие, требующее смены сцены

            //    var cmd = container.Resolve<ICommandProcessor>(); //Создаем обработчик команд
            var cmd = new CommandProcessorGameplay(gameStateProvider); //Создаем обработчик команд
            container.RegisterInstance<ICommandProcessor>(cmd); //Кешируем его в DI

            gameplayState.GameSpeed.Value =
                gameplayEnterParams.GameSpeed; //Получаем скорость игры из настроек GameState


            //cmd.RegisterHandler(new CommandPlaceBuildingHandler(gameState)); //Регистрируем команды обработки зданий
            cmd.RegisterHandler(new CommandPlaceTowerHandler(gameplayState));
            cmd.RegisterHandler(new CommandTowerBustHandler(gameplayState, gameSettings));
            cmd.RegisterHandler(new CommandCreateLevelHandler(gameSettings, gameplayState)); //Регистрируем команду создания уровня из конфигурации
            cmd.RegisterHandler(new CommandRewardKillMobHandler(gameplayState));
            cmd.RegisterHandler(new CommandDeleteTowerHandler(gameplayState));
            cmd.RegisterHandler(new CommandMoveTowerHandler(gameplayState));

            //Нужно загрузить карту, если ее нет, нужно брать по умолчанию
            if (gameplayState.Entities.Any() != true)
            {
                //Debug.Log(" Загружаем из настроек");
                var command = new CommandCreateLevel(gameplayEnterParams.MapId);
                var success = cmd.Process(command);
                if (!success)
                {
                    throw new Exception($"Карта не создалась с id = {gameplayEnterParams.MapId}");
                }
            }

            
            container.RegisterFactory(_ => new CastleService(
                gameplayState.Castle.Value,
                cmd)
            ).AsSingle();
            


            var placementService = new PlacementService(gameplayState);
            container.RegisterInstance(placementService);
            //Регистрируем сервис по Дорогам
            /*   container.RegisterFactory(_ => new RoadsService(
                   gameplayState.Entities,
                   gameSettings.RoadsSettings,
                   cmd,
                frameService)
               ).AsSingle();
               */
            container.RegisterFactory(_ => new GroundsService(
                gameplayState.Entities,
                cmd
                )
            ).AsSingle();

            var towersService = new TowersService(
                gameplayState.Entities,
                gameSettings.TowersSettings,
                cmd,
                placementService
            );
            
            container.RegisterInstance(towersService);
            var frameService = new FrameService(gameplayState, placementService, towersService);
            container.RegisterInstance(frameService);
           // container.RegisterFactory(_ => ).AsSingle();

            //Добавить сервисы и команды для
            /// Дорог
            /// Земли
            /// Монстров
            /// Башни вместо Здания


            Fsm.Fsm.SetState<FsmStateGamePlay>();


            //Регистрируем сервисы, завия
            var rewardService = new RewardProgressService(container);
            container.RegisterInstance(rewardService);

            container.RegisterFactory(_ => new GameplayService(subjectExitParams, container))
                .AsSingle(); //Сервис игры, следит, проиграли мы или нет, и создает выходные параметры
        }
    }
}