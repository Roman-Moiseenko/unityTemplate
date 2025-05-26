using DI;
using Game.GamePlay.Commands;
using Game.GamePlay.Services;
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
            var cmd = new CommandProcessor(gameStateProvider); //Создаем обработчик команд
            container.RegisterInstance<ICommandProcessor>(cmd); //Кешируем его в DI
            cmd.RegisterHandler(new CommandPlaceBuildingHandler(gameState)); //Регистрируем команды обработки зданий
            
            //Регистрируем сервис по Зданиями
            container.RegisterFactory(_ => new BuildingsService(gameState.Buildings,cmd)).AsSingle();
            
            //Добавить сервисы и команды для
            /// Дорог
            /// Земли
            /// Монстров
            /// Башни вместо Здания
        }
    }
}