using DI;
using Game.Common;
using Game.GamePlay.Commands;
using Game.MainMenu.Commands.ResourceCommands;
using Game.MainMenu.Services;
using Game.Settings;
using Game.State;
using Game.State.CMD;
using R3;
using Scripts.Game.GameRoot.Services;

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
            //Сервисы работы с карточками, кланом (присоединиться, запрос и др.) и другое

            container.RegisterFactory(c => new MainMenuExitParamsService(container)).AsSingle();
            container.RegisterFactory(_ => new ResourcesService(gameState.Resources, cmd)).AsSingle();
        }
    }
}