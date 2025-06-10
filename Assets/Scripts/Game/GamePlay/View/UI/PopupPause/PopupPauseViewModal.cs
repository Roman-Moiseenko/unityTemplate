using DI;
using Game.GamePlay.Root;
using Game.MainMenu.Root;
using MVVM.UI;
using R3;

namespace Game.GamePlay.View.UI.PopupPause
{
    /**
     * Окно при проигрыше 
     */
    public class PopupPauseViewModal : WindowViewModel
    {
        public override string Id => "PopupPause";
        public override string Path => "Gameplay/";
        private readonly Subject<GameplayExitParams> _exitSceneRequest;
        //TODO действия при нажатии на кнопки

        public PopupPauseViewModal(
            GameplayUIManager uiManager, 
            Subject<GameplayExitParams> exitSceneRequest,
            DIContainer container)
        {
            _exitSceneRequest = exitSceneRequest;
        }

        //Проигрыш 
        //Вызываем события закрытия окна с проигришем

        //

        //Вызываем событие просмотр рекламы, после завершения событие восстановления

        //Покупка восстановления ... отнимаем кристалы, событие восстановления
        public void RequestGoToMainMenu()
        {
            var menuParams = new MainMenuEnterParams("Выход");
            var exitParams = new GameplayExitParams(menuParams);
            _exitSceneRequest.OnNext(exitParams);
        }

        public void RequestExitSave()
        {
            var menuParams = new MainMenuEnterParams("Выход с сохранением");
            var exitParams = new GameplayExitParams(menuParams);
            exitParams.SaveGameplay = true;
            _exitSceneRequest.OnNext(exitParams);
        }
    }
}