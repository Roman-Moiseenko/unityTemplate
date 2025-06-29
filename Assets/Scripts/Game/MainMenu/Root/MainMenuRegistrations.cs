using System;
using System.Linq;
using DI;
using Game.Common;
using Game.GamePlay.Commands;
using Game.GamePlay.Commands.Inventory;
using Game.MainMenu.Commands.ResourceCommands;
using Game.MainMenu.Services;
using Game.Settings;
using Game.State;
using MVVM.CMD;
using R3;
using Scripts.Game.GameRoot.Services;
using UnityEngine;

namespace Game.MainMenu.Root
{
    /**
    * Регистрируем Сервисы сцены
    */
    public static class MainMenuRegistrations
    {
        public static void Register(DIContainer container, MainMenuEnterParams mainMenuEnterParams)
        {
            var gameStateProvider = container.Resolve<IGameStateProvider>(); //Получаем репозиторий
            var gameState = gameStateProvider.GameState;
            var settingsProvider = container.Resolve<ISettingsProvider>();
            var gameSettings = settingsProvider.GameSettings;

            var subjectExitParams = new Subject<MainMenuExitParams>();
            
        //    container.RegisterInstance(AppConstants.EXIT_SCENE_REQUEST_TAG,
               // new Subject<Unit>()); //Событие, требующее смены сцены
            container.RegisterInstance(subjectExitParams); //Событие, требующее смены сцены

          //  var cmd = container.Resolve<ICommandProcessor>();
            var cmd = new CommandProcessorMainMenu(gameStateProvider); //Создаем обработчик команд
            container.RegisterInstance<ICommandProcessor>(cmd); //Кешируем его в DI
            
            //TODO Командный процессор - команды работы с инвентарем
            cmd.RegisterHandler(new CommandResourcesAddHandler(gameState));
            cmd.RegisterHandler(new CommandResourcesSpendHandler(gameState));
            cmd.RegisterHandler(new CommandCreateInventoryHandler(gameState, gameSettings)); //Создание базового инвентаря
            //Сервисы работы с карточками, кланом (присоединиться, запрос и др.) и другое

            container.RegisterFactory(c => new MainMenuExitParamsService(container)).AsSingle();
            container.RegisterFactory(_ => new ResourcesService(gameState.Resources, cmd)).AsSingle();

            //Для нового игрока загружаем базовый инвентарь
            if (gameState.InventoryItems.Any() != true)
            {
                Debug.Log("Загружаем Инвентраь из Настроек");
                var command = new CommandCreateInventory();
                var success = cmd.Process(command);
                if (!success)
                {
                    throw new Exception($"Инвентарь не создался");
                }
            }
            //TODO Загружаем настройки и другое с сервера. Либо перенести в GameRoot 

            
            //var commandLoad = new CommandLoadSettings();
            //cmd.Process(commandLoad);
            
            //Сервисы карточек

            var towerCardService = new TowerCardService(
                gameState.InventoryItems,
                gameSettings.TowersSettings,
                cmd
                );
            container.RegisterInstance(towerCardService);
            
            
        }
    }
}