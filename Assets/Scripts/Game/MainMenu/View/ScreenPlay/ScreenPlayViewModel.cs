using Game.GamePlay.Root;
using Game.MainMenu.Root;
using MVVM.UI;
using R3;

namespace Game.MainMenu.View.ScreenPlay
{
    public class ScreenPlayViewModel : WindowViewModel
    {
        private readonly MainMenuUIManager _uiManager;
        private readonly Subject<MainMenuExitParams> _exitSceneRequest;
      //  private readonly Subject<MainMenuExitParams> _exitSceneRequest2 = new();
        public override string Id => "ScreenPlay";
        public override string Path => "MainMenu/";
        
        public ScreenPlayViewModel(MainMenuUIManager uiManager, Subject<MainMenuExitParams> exitSceneRequest)
        {
            _uiManager = uiManager;
            _exitSceneRequest = exitSceneRequest;
        }
        public void RequestBeginGame()
        {
            var _gameplayEnterParams = new GameplayEnterParams(0); //Получаем Id текущей карты
            //TODO Грузим данные для игры - бустеры, колоду и другое
            //Грузима карту из настроек
            var mainMenuExitParams = new MainMenuExitParams(_gameplayEnterParams);
            _exitSceneRequest.OnNext(mainMenuExitParams);
        }
        
        public void RequestResumeGame()
        {
            var _gameplayEnterParams = new GameplayEnterParams(0);
            //TODO Грузим данные для игры - бустеры, колоду и другое
            //TODO Надо загрузить данные из сейва карту и другие данные
            
            var mainMenuExitParams = new MainMenuExitParams(_gameplayEnterParams);
            
           // _exitSceneRequest2.OnNext(mainMenuExitParams);
            _exitSceneRequest.OnNext(mainMenuExitParams);
        }
    }
}