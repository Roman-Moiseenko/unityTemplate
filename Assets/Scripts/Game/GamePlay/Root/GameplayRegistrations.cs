using System;
using System.Linq;
using DI;
using Game.Common;
using Game.GamePlay.Commands;
using Game.GamePlay.Commands.MapCommand;
using Game.GamePlay.Commands.TowerCommand;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.GamePlay.Services;
using Game.Settings;
using Game.State;
using Game.State.CMD;
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
            var gameState = gameStateProvider.GameState;
            var gameplayState = gameStateProvider.GameplayState;
            var settingsProvider = container.Resolve<ISettingsProvider>();
            var gameSettings = settingsProvider.GameSettings;
            //Регистрируем машину состояния
            var Fsm = new FsmGameplay(container);
            container.RegisterInstance(Fsm);
            
            container.RegisterInstance(AppConstants.EXIT_SCENE_REQUEST_TAG, new Subject<GameplayExitParams>()); //Событие, требующее смены сцены
           // container.RegisterInstance(AppConstants.GAME_PLAY_STATE, new Subject<Unit>()); //Событие, меняющее состояние игры

            var cmd = container.Resolve<ICommandProcessor>(); // new CommandProcessor(gameStateProvider); //Создаем обработчик команд
            //container.RegisterInstance<ICommandProcessor>(cmd); //Кешируем его в DI
            
            //cmd.RegisterHandler(new CommandPlaceBuildingHandler(gameState)); //Регистрируем команды обработки зданий
            //cmd.RegisterHandler(new CommandPlaceTowerHandler(gameState));
            cmd.RegisterHandler(new CommandCreateMapHandler(gameState, gameSettings, gameplayState)); //Регистрируем команды обработки зданий

            //TODO CommandProcessor и команды Resources регистрировать раньше, т.к. используются в меню 
            //TODO либо делать 2 уровня ресурсов - ОбщеИгровые и Игровые (сессионные) 
            cmd.RegisterHandler(new CommandResourcesAddHandler(gameState));
            cmd.RegisterHandler(new CommandResourcesSpendHandler(gameState));
            
            //Нужно загрузить карту, если ее нет, нужно брать по умолчанию
            var loadingMapId = gameplayEnterParams.MapId;
            var loadingMap = gameState.Maps.FirstOrDefault(m => m.Id == loadingMapId);

          /*  if (gameplayState.En)
            {
                
            }*/
            //var loadingMap = gameplayState;
           // Debug.Log("loadingMapId " + loadingMapId);
            
            if (loadingMap == null)
            {
                var command = new CommandCreateMap(loadingMapId);
                var success = cmd.Process(command);
                if (!success)
                {
                    throw new Exception($"Карта не создалась с id = {loadingMapId}");
                }

                loadingMap = gameState.Maps.First(m => m.Id == loadingMapId); //??
                //Debug.Log("loadingMap: " + JsonConvert.SerializeObject(loadingMap, Formatting.Indented));
            }
           // Debug.Log("*** gameSettings: " + JsonConvert.SerializeObject(gameState, Formatting.Indented));

            //Регистрируем сервис по Зданиями
               container.RegisterFactory(_ => new BuildingsService(
                   loadingMap.Entities,
                   gameSettings.BuildingsSettings,
                   cmd)
               ).AsSingle();
               container.RegisterFactory(_ => new GroundsService(
                   loadingMap.Entities,
                   cmd)
               ).AsSingle();
               
               container.RegisterFactory(_ => new TowersService(
                   loadingMap.Entities,
                   gameSettings.TowersSettings,
                   cmd
                   )
               ).AsSingle();

               container.RegisterFactory(_ => new ResourcesService(gameState.Resources, cmd)).AsSingle();
               
               //Добавить сервисы и команды для
               /// Дорог
               /// Земли
               /// Монстров
               /// Башни вместо Здания


               Fsm.Fsm.SetState<FsmStateGamePlay>();
               
               
                
               //Регистрируем сервисы, завия
               var rewardService = new RewardProgressService(container);
               container.RegisterInstance(rewardService);
               
        }
    }
}