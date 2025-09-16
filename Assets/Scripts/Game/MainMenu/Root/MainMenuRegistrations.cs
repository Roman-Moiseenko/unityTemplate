using System;
using System.Linq;
using DI;
using Game.MainMenu.Commands.ChestCommands;
using Game.MainMenu.Commands.InventoryCommands;
using Game.MainMenu.Commands.ResourceCommands;
using Game.MainMenu.Commands.SoftCurrency;
using Game.MainMenu.Commands.TowerCommands;
using Game.MainMenu.Services;
using Game.MainMenu.View.ScreenInventory.TowerCards;
using Game.MainMenu.View.ScreenInventory.TowerPlans;
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

            //HРЕГИСТРИРУЕМ СОБЫТИЯ
            var subjectExitParams = new Subject<MainMenuExitParams>();
            container.RegisterFactory(_ => new Subject<TowerCardViewModel>()).AsSingle();
            container.RegisterFactory(_ => new Subject<TowerPlanViewModel>()).AsSingle();


            // new Subject<Unit>()); //Событие, требующее смены сцены
            container.RegisterInstance(subjectExitParams); //Событие, требующее смены сцены

            //  var cmd = container.Resolve<ICommandProcessor>();
            var cmd = container.Resolve<ICommandProcessor>();
            var chestService = new ChestService(gameState, cmd, gameSettings);
            container.RegisterInstance(chestService);

            //команды работы с инвентарем
            //CHEST
            cmd.RegisterHandler(new CommandChestOpeningHandler(gameState));
            cmd.RegisterHandler(new CommandChestAddHandler(gameState));
            cmd.RegisterHandler(new CommandChestOpenedHandler(gameState));
            cmd.RegisterHandler(new CommandChestOpenHandler(gameState, cmd));
            cmd.RegisterHandler(new CommandInventoryFromRewardHandler(cmd));
            cmd.RegisterHandler(new CommandChestForcedHandler(gameState));
            //INVENTORY
            cmd.RegisterHandler(new CommandCreateInventoryHandler(gameState, gameSettings, cmd));
            cmd.RegisterHandler(new CommandInventoryItemAddHandler(gameState));
            cmd.RegisterHandler(new CommandInventoryItemSpendHandler(gameState));
            //RESOURCE
            cmd.RegisterHandler(new CommandResourcesAddHandler(gameState));
            cmd.RegisterHandler(new CommandResourcesSpendHandler(gameState));
            //CURRENCY
            cmd.RegisterHandler(new CommandSoftCurrencyAddHandler(gameState));
            cmd.RegisterHandler(new CommandSoftCurrencySpendHandler(gameState));
            //TOWER
            cmd.RegisterHandler(new CommandTowerCardLevelUpHandler(gameState));
            cmd.RegisterHandler(new CommandTowerCardAddHandler(gameState, gameSettings));
            cmd.RegisterHandler(new CommandTowerCardSpendHandler(gameState));

            cmd.RegisterHandler(new CommandTowerPlanAddHandler(gameState));
            cmd.RegisterHandler(new CommandTowerPlanSpendHandler(gameState));

            
            
            //
            container.RegisterFactory(_ => new InventoryService(cmd, gameState, chestService)).AsSingle();

            container.RegisterFactory(c => new MainMenuExitParamsService(container)).AsSingle();
            //container.RegisterFactory(_ => new ResourcesService(gameState.Resources, cmd)).AsSingle();

            //  Debug.Log(JsonConvert.SerializeObject(gameState.Inventory.TowerCardBag.Items, Formatting.Indented));
            //Для нового игрока загружаем базовый инвентарь
            if (gameState.Inventory.Items.Any() != true)
            {
                // Debug.Log("Загружаем Инвентарь из Настроек");
                var command = new CommandCreateInventory();
                var success = cmd.Process(command);
                if (!success)
                {
                    throw new Exception($"Инвентарь не создался");
                }
                //Debug.Log(JsonConvert.SerializeObject(gameState.Inventory.DeckCards, Formatting.Indented));
                //Debug.Log(JsonConvert.SerializeObject(gameState.Inventory.BattleDeck, Formatting.Indented));
            }
            //TODO Загружаем настройки и другое с сервера. Либо перенести в GameRoot 

            //Сервисы карточек

            var towerCardService = new TowerCardPlanService(
                gameState.Inventory,
                gameSettings.TowersSettings,
                cmd,
                container
            );
            container.RegisterInstance(towerCardService);
            //TODO Запускаем первоначальные проверки
            chestService.StartOpeningChests(); //Проверка на открывающиеся и открытые сундуки, меняем TimerLeft
            
            //var commandOpened = new CommandChestOpened();
            //cmd.Process(commandOpened);
        }
    }
}