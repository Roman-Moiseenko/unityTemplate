using System;
using System.Linq;
using DI;
using Game.Common;
using Game.GamePlay.Commands;
using Game.GamePlay.Commands.Inventory;
using Game.MainMenu.Commands.InventoryCommands;
using Game.MainMenu.Commands.ResourceCommands;
using Game.MainMenu.Commands.SoftCurrency;
using Game.MainMenu.Commands.TowerCommands;
using Game.MainMenu.Services;
using Game.Settings;
using Game.State;
using MVVM.CMD;
using Newtonsoft.Json;
using R3;
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
            var cmd = container.Resolve<ICommandProcessor>();
            //container.RegisterInstance<ICommandProcessor>(cmd); //Кешируем его в DI

            //команды работы с инвентарем
            cmd.RegisterHandler(new CommandResourcesAddHandler(gameState));
            cmd.RegisterHandler(new CommandResourcesSpendHandler(gameState));
            cmd.RegisterHandler(
                new CommandCreateInventoryHandler(gameState, gameSettings)); //Создание базового инвентаря
            //Сервисы работы с карточками, кланом (присоединиться, запрос и др.) и другое
            cmd.RegisterHandler(new CommandInventoryItemAddHandler(gameState));
            cmd.RegisterHandler(new CommandInventoryItemSpendHandler(gameState));
            
            cmd.RegisterHandler(new CommandSoftCurrencySpendHandler(gameState));
            cmd.RegisterHandler(new CommandTowerCardLevelUpHandler(gameState));

            container.RegisterFactory(c => new MainMenuExitParamsService(container)).AsSingle();
            //container.RegisterFactory(_ => new ResourcesService(gameState.Resources, cmd)).AsSingle();

          //  Debug.Log(JsonConvert.SerializeObject(gameState.Inventory.TowerCardBag.Items, Formatting.Indented));
            //Для нового игрока загружаем базовый инвентарь
            if (gameState.Inventory.Items.Any() != true)
            {
                Debug.Log("Загружаем Инвентарь из Настроек");
                var command = new CommandCreateInventory();
                var success = cmd.Process(command);
                if (!success)
                {
                    throw new Exception($"Инвентарь не создался");
                }
            //    Debug.Log(JsonConvert.SerializeObject(gameState.Inventory.Origin.TowerCardBag.Items, Formatting.Indented));
            }
            //TODO Загружаем настройки и другое с сервера. Либо перенести в GameRoot 

            
            //var commandLoad = new CommandLoadSettings();
            //cmd.Process(commandLoad);

            //Сервисы карточек

            var towerCardService = new TowerCardService(
                gameState.Inventory,
                gameSettings.TowersSettings,
                cmd
            );
            container.RegisterInstance(towerCardService);
        }
    }
}