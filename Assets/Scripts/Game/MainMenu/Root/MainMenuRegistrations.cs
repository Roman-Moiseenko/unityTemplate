using DI;
using Game.Common;
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

            
        //    container.RegisterInstance(AppConstants.EXIT_SCENE_REQUEST_TAG,
               // new Subject<Unit>()); //Событие, требующее смены сцены
            container.RegisterInstance(AppConstants.EXIT_SCENE_REQUEST_TAG,
                new Subject<MainMenuExitParams>()); //Событие, требующее смены сцены

            var cmd = container.Resolve<ICommandProcessor>();
            //TODO Командный процессор - команды работы с инвентарем

            //Сервисы работы с карточками, кланом (присоединиться, запрос и др.) и другое

            container.RegisterFactory(c => new MainMenuExitParamsService(container)).AsSingle();
        }
    }
}