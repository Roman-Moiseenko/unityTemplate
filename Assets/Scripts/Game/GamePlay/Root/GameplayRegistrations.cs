using System;
using System.Collections;
using System.Linq;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Commands;
using Game.GamePlay.Commands.CastleCommands;
using Game.GamePlay.Commands.GroundCommands;
using Game.GamePlay.Commands.MapCommand;
using Game.GamePlay.Commands.RewardCommand;
using Game.GamePlay.Commands.RoadCommand;
using Game.GamePlay.Commands.TowerCommand;
using Game.GamePlay.Commands.WaveCommands;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.GamePlay.Services;
using Game.GameRoot.Services;
using Game.MainMenu.Services;
using Game.Settings;
using Game.State;
using MVVM.CMD;
using MVVM.FSM;
using Newtonsoft.Json;
using R3;
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
           // var subjectExitParams = new Subject<GameplayExitParams>();
          //  container.RegisterInstance(subjectExitParams); //Событие, требующее смены сцены
          //  var generateService = new GenerateService();
           // container.RegisterInstance(generateService);
            //    var cmd = container.Resolve<ICommandProcessor>(); //Создаем обработчик команд
            var cmd = container.Resolve<ICommandProcessor>();
            //var cmd = new CommandProcessorGameplay(gameStateProvider); //Создаем обработчик команд
            //container.RegisterInstance<ICommandProcessor>(cmd); //Кешируем его в DI

            gameplayState.GameSpeed.Value =
                gameplayEnterParams.GameSpeed; //Получаем скорость игры из настроек GameState


            cmd.RegisterHandler(new CommandCreateGroundHandler(gameplayState));
            cmd.RegisterHandler(new CommandPlaceTowerHandler(gameplayState, gameSettings.TowersSettings));
            cmd.RegisterHandler(new CommandTowerLevelUpHandler(gameplayState, gameSettings));
            cmd.RegisterHandler(new CommandCreateWaveHandler(gameSettings, gameplayState, 
                container.Resolve<GenerateService>()));
            cmd.RegisterHandler(new CommandWaveGenerateHandler(gameSettings, cmd, 
                            container.Resolve<GenerateService>()));
            
            cmd.RegisterHandler(new CommandCastleCreateHandler(gameSettings, gameplayState));
            cmd.RegisterHandler(new CommandGroundCreateBaseHandler(cmd));
            cmd.RegisterHandler(new CommandRoadCreateBaseHandler(gameplayState, cmd));
            //Команды создания карт
            cmd.RegisterHandler(new CommandCreateLevelHandler(gameSettings, gameplayState, cmd)); // по уровням 
            cmd.RegisterHandler(new CommandCreateInfinityHandler(gameSettings, gameplayState, cmd)); // бесконечный            
            //cmd.RegisterHandler(new CommandCreateEventHandler(gameSettings, gameplayState, cmd)); //евенты            
            
            cmd.RegisterHandler(new CommandRewardKillMobHandler(gameplayState));
            cmd.RegisterHandler(new CommandDeleteTowerHandler(gameplayState));
            cmd.RegisterHandler(new CommandMoveTowerHandler(gameplayState));
            cmd.RegisterHandler(new CommandPlaceRoadHandler(gameplayState));

            var newMapSettings = gameSettings.MapsSettings.Maps.First(m => m.MapId == gameplayEnterParams.MapId);
            var groundConfigId = newMapSettings.InitialStateSettings.groundDefault;
            var roadConfigId = newMapSettings.InitialStateSettings.roadDefault;


            var wayService = new WayService(); //Сервис обсчета дороги
            container.RegisterInstance(wayService);

            var placementService = new PlacementService(gameplayState, wayService);
            container.RegisterInstance(placementService);

            var roadsService = new RoadsService(
                gameplayState.Way,
                gameplayState.WaySecond,
                gameplayState.WayDisabled,
                roadConfigId,
                cmd);
            //Регистрируем сервис по Дорогам
            container.RegisterInstance(roadsService);

            //Сервис по земле
            container.RegisterFactory(_ => new GroundsService(
                    gameplayState.Grounds,
                    groundConfigId,
                    cmd
                )
            ).AsSingle();

            //Сервис башен
            var towersService = new TowersService(
                gameplayState.Towers,
                gameSettings.TowersSettings,
                gameplayEnterParams.Towers,
                cmd,
                placementService
            );

            container.RegisterInstance(towersService);

            var frameService = new FrameService(gameplayState, placementService, towersService, roadsService,
                gameSettings.TowersSettings);
            container.RegisterInstance(frameService);
            // container.RegisterFactory(_ => ).AsSingle();
            container.RegisterFactory(_ => new GameplayCamera(container)).AsSingle();
            //сервис волн мобов
            var waveService = new WaveService(container, gameplayState);
            container.RegisterInstance(waveService);

            container.RegisterFactory(_ => new CastleService(container,
                gameplayState.Castle, gameplayState)).AsSingle();

           // Fsm.Fsm.SetState<FsmStateGamePlay>();

            //Сервис наград
            var rewardService = new RewardProgressService(gameplayState, container, gameSettings.TowersSettings);
            container.RegisterInstance(rewardService);

            var gameplayService = new GameplayService(
                container.Resolve<Subject<GameplayExitParams>>(), 
                waveService, gameplayState,
                container.Resolve<AdService>(),
                Fsm,
                container.Resolve<ResourceService>(),
                cmd
                );
            container.RegisterInstance(gameplayService); //Сервис игры, следит, проиграли мы или нет, и создает выходные параметры
            //Сервис создания выстрелов
            var shotService = new ShotService(gameplayState, gameSettings.TowersSettings, Fsm);
            container.RegisterInstance(shotService);

            var damageService = new DamageService(Fsm, gameplayState, gameSettings.TowersSettings, waveService,
                towersService, shotService, rewardService);

            container.RegisterInstance(damageService);

            //Загружаем уровень из настроек, если gameplayState пуст.
            if (gameplayState.Towers.Any() != true)
            {
                var success = false;
                switch (gameplayEnterParams.TypeGameplay)
                {
                    case TypeGameplay.Infinity:
                    {
                        var command = new CommandCreateInfinity(gameplayEnterParams.MapId);
                        success = cmd.Process(command);
                        break;
                    }
                    case TypeGameplay.Levels:
                    {
                        var command = new CommandCreateLevel(gameplayEnterParams.MapId);
                        success = cmd.Process(command);
                        break;
                    }
                    case TypeGameplay.Event:
                        break;
                    case TypeGameplay.Resume:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                if (!success) throw new Exception($"Карта не создалась с id = {gameplayEnterParams.MapId}");
                Fsm.Fsm.SetState<FsmStateBuildBegin>(); //Устанавливаем начальный режим строительства
                
            }
            
            waveService.Start(); //Запуск таймера
        }


        public static IEnumerator RegisterCoroutine(DIContainer container, GameplayEnterParams gameplayEnterParams)
        {
            yield return null;
        }
    }
}