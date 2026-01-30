using System;
using System.Linq;
using DI;
using Game.GamePlay.Classes;
using Game.GamePlay.Commands.CastleCommands;
using Game.GamePlay.Commands.GroundCommands;
using Game.GamePlay.Commands.MapCommand;
using Game.GamePlay.Commands.MobCommands;
using Game.GamePlay.Commands.RewardCommand;
using Game.GamePlay.Commands.RoadCommand;
using Game.GamePlay.Commands.TowerCommand;
using Game.GamePlay.Commands.WarriorCommands;
using Game.GamePlay.Commands.WaveCommands;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.GameplayStates;
using Game.GamePlay.Queries.TowerQueries;
using Game.GamePlay.Queries.WaveQueries;
using Game.GamePlay.Services;
using Game.GameRoot.Services;
using Game.Settings;
using Game.State;
using MVVM.CMD;
using Newtonsoft.Json;
using R3;
using UnityEngine;
using Random = System.Random;

namespace Game.GamePlay.Root
{
    public static class GameplayRegistrations
    {
        /**
         * Регистрируем все сервисы для сцены, вручную
         */
        public static void Register(DIContainer container, GameplayEnterParams gameplayEnterParams)
        {
            var defaultGroundConfigId = "";
            var defaultRoadConfigId = "";
            //Загружаем параметры карт от типа игры 
            var gameStateProvider = container.Resolve<IGameStateProvider>(); //Получаем репозиторий
            var gameState = gameStateProvider.GameState; //TODO Получим кристалы для изменения


            var gameplayState = gameStateProvider.GameplayState;
            gameplayState.MapId.OnNext(gameplayEnterParams.MapId);
            gameplayState.TypeGameplay.OnNext(gameplayEnterParams.TypeGameplay);

            var settingsProvider = container.Resolve<ISettingsProvider>();
            var gameSettings = settingsProvider.GameSettings;

            //Регистрируем машину состояния
            var fsmGameplay = new FsmGameplay(container);
            container.RegisterInstance(fsmGameplay);
            var fsmWave = new FsmWave(container);
            container.RegisterInstance(fsmWave);
            var fsmTower = new FsmTower(container);
            container.RegisterInstance(fsmTower);

            switch (gameplayState.TypeGameplay.CurrentValue)
            {
                case TypeGameplay.Infinity:
                {
                    var random = new Random();
                    var indexGround = random.Next(gameSettings.MapsSettings.GroundConfigIds.Count);
                    defaultGroundConfigId = gameSettings.MapsSettings.GroundConfigIds[indexGround];
                    var indexRoad = random.Next(gameSettings.MapsSettings.RoadConfigIds.Count);
                    defaultRoadConfigId = gameSettings.MapsSettings.RoadConfigIds[indexRoad];
                    break;
                }
                case TypeGameplay.Levels:
                {
                    var newMapSettings =
                        gameSettings.MapsSettings.Maps.Find(m => m.MapId == gameplayEnterParams.MapId);
                    if (newMapSettings == null)
                        throw new Exception("Нет настроек для карты " + gameplayEnterParams.MapId);
                    defaultGroundConfigId = newMapSettings.InitialStateSettings.groundDefault;
                    defaultRoadConfigId = newMapSettings.InitialStateSettings.roadDefault;
                    break;
                }
                case TypeGameplay.Event:

                    break;
                case TypeGameplay.Resume:

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var qrc = container.Resolve<IQueryProcessor>();
            qrc.RegisterHandler(new QueryInfoWaveHandler(gameplayState));
            qrc.RegisterHandler(new QueryInfoTowerHandler(gameplayState));

            // var subjectExitParams = new Subject<GameplayExitParams>();
            //  container.RegisterInstance(subjectExitParams); //Событие, требующее смены сцены
            //  var generateService = new GenerateService();
            // container.RegisterInstance(generateService);
            //    var cmd = container.Resolve<ICommandProcessor>(); //Создаем обработчик команд
            var cmd = container.Resolve<ICommandProcessor>();
            //var cmd = new CommandProcessorGameplay(gameStateProvider); //Создаем обработчик команд
            //container.RegisterInstance<ICommandProcessor>(cmd); //Кешируем его в DI

            cmd.RegisterHandler(new CommandCreateGroundHandler(gameplayState));
            cmd.RegisterHandler(new CommandPlaceTowerHandler(gameplayState, gameSettings.TowersSettings, cmd));
            cmd.RegisterHandler(new CommandTowerLevelUpHandler(gameplayState, gameSettings));
            cmd.RegisterHandler(new CommandCreateWaveHandler(gameSettings, gameplayState, cmd));
            cmd.RegisterHandler(new CommandCreateMobHandler(gameSettings, gameplayState));
            
            //Бесконечная игра, пока не используется
            //cmd.RegisterHandler(new CommandWaveGenerateHandler(gameSettings, cmd, container.Resolve<GenerateService>()));
            
            cmd.RegisterHandler(new CommandCastleCreateHandler(gameSettings, gameplayState));
            cmd.RegisterHandler(new CommandGroundCreateBaseHandler(cmd));
            cmd.RegisterHandler(new CommandRoadCreateBaseHandler(gameplayState, cmd));
            //Команды создания карт
            cmd.RegisterHandler(new CommandCreateLevelHandler(gameSettings, gameplayState, cmd)); // по уровням 

            if (gameplayEnterParams.TypeGameplay == TypeGameplay.Infinity)
                cmd.RegisterHandler(new CommandCreateInfinityHandler(gameSettings, gameplayState,
                    cmd, defaultGroundConfigId, defaultRoadConfigId)); // бесконечный            
            //cmd.RegisterHandler(new CommandCreateEventHandler(gameSettings, gameplayState, cmd)); //евенты            

            cmd.RegisterHandler(new CommandRewardKillMobHandler(gameplayState));
            cmd.RegisterHandler(new CommandDeleteTowerHandler(gameplayState));
            cmd.RegisterHandler(new CommandMoveTowerHandler(gameplayState));
            cmd.RegisterHandler(new CommandPlaceRoadHandler(gameplayState));
            //var newMapSettings = gameSettings.MapsSettings.Maps.First(m => m.MapId == gameplayEnterParams.MapId);
            var wayService = new WayService(); //Сервис обсчета дороги
            container.RegisterInstance(wayService);

            var placementService = new PlacementService(gameplayState, wayService);
            container.RegisterInstance(placementService);

            var roadsService = new RoadsService(
                gameplayState.Way,
                gameplayState.WaySecond,
                gameplayState.WayDisabled,
                defaultRoadConfigId,
                cmd);
            //Регистрируем сервис по Дорогам
            container.RegisterInstance(roadsService);

            //Сервис по земле
            container.RegisterFactory(_ => new GroundsService(
                    gameplayState.Grounds,
                    defaultGroundConfigId,
                    cmd,
                    gameplayState
                )
            ).AsSingle();

            //Сервис воинов (нужен для башен и навыков)
            var warriorService = new WarriorService(gameplayState, cmd);
            container.RegisterInstance(warriorService);
            //Сервис башен
            var towersService = new TowersService(
                gameplayState,
                gameSettings.TowersSettings,
                gameplayEnterParams.Towers,
                cmd,
                placementService,
                warriorService,
                fsmTower
            );

            container.RegisterInstance(towersService);
            
            cmd.RegisterHandler(new CommandCreateWarriorTowerHandler(gameplayState, towersService));
            cmd.RegisterHandler(new CommandRemoveWarriorTowerHandler(gameplayState));
            
            var frameService = new FrameService(gameplayState, placementService, towersService, roadsService,
                gameSettings.TowersSettings, qrc);
            container.RegisterInstance(frameService);

            container.RegisterFactory(_ => new GameplayCamera(container)).AsSingle();
            //сервис волн мобов
            var waveService = new WaveService(container, gameplayState, cmd);
            container.RegisterInstance(waveService);

            container.RegisterFactory(_ => new CastleService(container,
                gameplayState.Castle, gameplayState)).AsSingle();


            //Сервис наград
            var rewardService = new RewardProgressService(gameplayState, container, gameSettings);
            container.RegisterInstance(rewardService);

            var gameplayService = new GameplayService(
                container.Resolve<Subject<GameplayExitParams>>(),
                gameplayState,
                container.Resolve<AdService>(),
                container.Resolve<ResourceService>(),
                cmd,
                gameState,
                gameSettings.MapsSettings,
                towersService
            );
            //Сервис игры, следит, проиграли мы или нет, и создает выходные параметры
            container.RegisterInstance(gameplayService);
            
            container.RegisterFactory(_ => new DamageService(gameplayState)).AsSingle();

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
                fsmGameplay.Fsm.SetState<FsmStateBuildBegin>(); //Устанавливаем начальный режим строительства
            }
            //Debug.Log(JsonConvert.SerializeObject(gameplayState.Waves, Formatting.Indented));
        }
    }
}