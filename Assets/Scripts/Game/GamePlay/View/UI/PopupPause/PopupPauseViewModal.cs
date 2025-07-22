using DI;
using Game.GamePlay.Root;
using Game.GamePlay.Services;
using Game.MainMenu.Root;
using Game.State;
using Game.State.Root;
using MVVM.UI;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.UI.PopupPause
{
    /**
     * Окно при проигрыше 
     */
    public class PopupPauseViewModal : WindowViewModel
    {
        public override string Id => "PopupPause";
        public override string Path => "Gameplay/Popups/";
        private readonly Subject<GameplayExitParams> _exitSceneRequest;
        private readonly DIContainer _container;

        private readonly GameplayStateProxy _gameplayState;
        //TODO действия при нажатии на кнопки

        public PopupPauseViewModal(
            GameplayUIManager uiManager, 
            Subject<GameplayExitParams> exitSceneRequest,
            DIContainer container)
        {
            _exitSceneRequest = exitSceneRequest;
            _container = container;

            _gameplayState = container.Resolve<IGameStateProvider>().GameplayState;
        }

        //Проигрыш 
        //Вызываем события закрытия окна с проигришем

        //

        //Вызываем событие просмотр рекламы, после завершения событие восстановления

        //Покупка восстановления ... отнимаем кристалы, событие восстановления
        public void RequestGoToMainMenu()
        {
            _container.Resolve<GameplayService>().Lose();
        }

        public void RequestExitSave()
        {
            _container.Resolve<GameplayService>().ExitSave();
        }
    }
}