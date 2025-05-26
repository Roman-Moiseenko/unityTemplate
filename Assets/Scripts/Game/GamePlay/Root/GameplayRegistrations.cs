using System;
using System.Linq;
using DI;
using Game.GamePlay.Commands;
using Game.GamePlay.Services;
using Game.Settings;
using Game.State;
using Game.State.CMD;
using Scripts.Game.GameRoot.Services;

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
            var settingsProvider = container.Resolve<ISettingsProvider>();
            var gameSettings = settingsProvider.GameSettings;
            var cmd = new CommandProcessor(gameStateProvider); //Создаем обработчик команд
            container.RegisterInstance<ICommandProcessor>(cmd); //Кешируем его в DI
            cmd.RegisterHandler(new CommandPlaceBuildingHandler(gameState)); //Регистрируем команды обработки зданий
            cmd.RegisterHandler(new CommandCreateMapStateHandler(gameState, gameSettings)); //Регистрируем команды обработки зданий

            //Нужно загрузить карту, если ее нет, нужно брать по умолчанию
            var loadingMapId = gameplayEnterParams.MapId;
            var loadingMap = gameState.Maps.FirstOrDefault(m => m.Id == loadingMapId);
            if (loadingMap == null)
            {
                var command = new CommandCreateMapState(loadingMapId);
                var success = cmd.Process(command);
                if (!success)
                {
                    throw new Exception($"Карта не создалась с id = {loadingMapId}");
                }

                loadingMap = gameState.Maps.First(m => m.Id == loadingMapId); //??
            }

            //Регистрируем сервис по Зданиями
               container.RegisterFactory(_ => new BuildingsService(
                   loadingMap.Buildings,
                   gameSettings.BuildingsSettings,
                   cmd)
               ).AsSingle();

            //Добавить сервисы и команды для
            /// Дорог
            /// Земли
            /// Монстров
            /// Башни вместо Здания
        }
    }
}