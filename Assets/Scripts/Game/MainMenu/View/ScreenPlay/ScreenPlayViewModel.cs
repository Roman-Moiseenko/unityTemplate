using DI;
using Game.GamePlay.Classes;
using Game.GamePlay.Root;
using Game.MainMenu.Root;
using Game.MainMenu.Services;
using Game.MainMenu.View.ScreenPlay.Chests;
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

        public ChestsViewModel ChestsViewModel;


        //  private readonly Subject<MainMenuExitParams> _exitSceneRequest2 = new();
        public override string Id => "ScreenPlay";
        public override string Path => "MainMenu/ScreenPlay/";
        
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
            var gameState = container.Resolve<IGameStateProvider>().GameState;
            ChestsViewModel = new ChestsViewModel(gameState, container);

        }

        
        //TODO Получаем список карт по уровню, доступные и номер последней. Остальные  для просмотра
        //TODO При выборе, сохраняем его уровень - MapId
        //TODO в RequestBeginGame его передаем или берем из стека
        
        public void RequestBeginGame()
        {
            //TODO Получить из GameState текущую карту
            //Грузим данные для игры - бустеры, колоду и другое
            var mainMenuExitParams = _exitParamsService.GetExitParams(TypeGameplay.Levels, 0);
            //TODO Если осталась сессия, то сброс ее ... Перенести в сервис
            _container.Resolve<IGameStateProvider>().ResetGameplayState();
            _exitSceneRequest.OnNext(mainMenuExitParams);
        }

        public bool RequestInfinityGame()
        {
            var mainMenuExitParams = _exitParamsService.GetExitParams(TypeGameplay.Infinity, 0);
            //TODO проверка на колоду return false;
            _container.Resolve<IGameStateProvider>().ResetGameplayState();
            _exitSceneRequest.OnNext(mainMenuExitParams);
            return true;

        }
        
        /**
         * Временная ф-ция, перейдет в попап подтверждения возврата к игре
         */
        public void RequestResumeGame()
        {
            //Грузим данные для игры - бустеры, колоду и другое
            var mainMenuExitParams = _exitParamsService.GetExitParams(TypeGameplay.Resume, 0);
           // mainMenuExitParams.TargetSceneEnterParams.As<GameplayEnterParams>().HasSessionGameplay = true;

            _exitSceneRequest.OnNext(mainMenuExitParams);
        }
    }
}