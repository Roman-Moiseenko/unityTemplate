using DI;
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
        private readonly Subject<Unit> _exitSceneRequest;
        //TODO действия при нажатии на кнопки

        public PopupPauseViewModal(
            GameplayUIManager uiManager, 
            Subject<Unit> exitSceneRequest,
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
            _exitSceneRequest.OnNext(Unit.Default);
        }
    }
}