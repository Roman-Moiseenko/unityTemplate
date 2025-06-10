using DI;
using Game.GamePlay.Root;
using Game.MainMenu.Root;
using Game.MainMenu.Services;
using Game.State;
using MVVM.UI;
using R3;

namespace Game.MainMenu.View.ScreenPlay
{
    public class ScreenPlayViewModel : WindowViewModel
    {
        private readonly MainMenuUIManager _uiManager;
        private readonly Subject<MainMenuExitParams> _exitSceneRequest;
        private readonly MainMenuExitParamsService _exitParamsService;
        private readonly DIContainer _container;


        //  private readonly Subject<MainMenuExitParams> _exitSceneRequest2 = new();
        public override string Id => "ScreenPlay";
        public override string Path => "MainMenu/";
        
        public ScreenPlayViewModel(
            MainMenuUIManager uiManager, 
            Subject<MainMenuExitParams> exitSceneRequest,
            MainMenuExitParamsService exitParamsService,
            DIContainer container
            )
        {
            _uiManager = uiManager;
            _exitSceneRequest = exitSceneRequest;
            _exitParamsService = exitParamsService;
            _container = container;
        }
        public void RequestBeginGame()
        {
            //TODO Получить из GameState текущую карту
            //Грузим данные для игры - бустеры, колоду и другое
            var mainMenuExitParams = _exitParamsService.GetExitParams(0);
            //TODO Если осталась сессия, то сброс ее ... Перенести в сервис
            _container.Resolve<IGameStateProvider>().ResetGameplayState();
            _exitSceneRequest.OnNext(mainMenuExitParams);
        }
        
        /**
         * Временная ф-ция, перейдет в попап подтверждения возврата к игре
         */
        public void RequestResumeGame()
        {
            //Грузим данные для игры - бустеры, колоду и другое
            var mainMenuExitParams = _exitParamsService.GetExitParams(0);
           // mainMenuExitParams.TargetSceneEnterParams.As<GameplayEnterParams>().HasSessionGameplay = true;

            _exitSceneRequest.OnNext(mainMenuExitParams);
        }
    }
}