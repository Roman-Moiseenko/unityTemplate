using DI;
using Game.GamePlay.Root;
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
        public override string Path => "Gameplay/";
        private readonly Subject<GameplayExitParams> _exitSceneRequest;

        private readonly GameplayStateProxy _gameplayState;
        //TODO действия при нажатии на кнопки

        public PopupPauseViewModal(
            GameplayUIManager uiManager, 
            Subject<GameplayExitParams> exitSceneRequest,
            DIContainer container)
        {
            _exitSceneRequest = exitSceneRequest;

            _gameplayState = container.Resolve<IGameStateProvider>().GameplayState;
        }

        //Проигрыш 
        //Вызываем события закрытия окна с проигришем

        //

        //Вызываем событие просмотр рекламы, после завершения событие восстановления

        //Покупка восстановления ... отнимаем кристалы, событие восстановления
        public void RequestGoToMainMenu()
        {
            //TODO Перенести в GameplayService
            var menuParams = new MainMenuEnterParams("Выход");
            //Передаем награду и некоторые настройки, для загрузки в меню
            menuParams.GameSpeed = _gameplayState.PreviousGameSpeed;
            menuParams.SoftCurrency = _gameplayState.SoftCurrency.CurrentValue;
            var exitParams = new GameplayExitParams(menuParams);
            _exitSceneRequest.OnNext(exitParams);
        }

        public void RequestExitSave()
        {
            var menuParams = new MainMenuEnterParams("Выход с сохранением");
            menuParams.GameSpeed = _gameplayState.PreviousGameSpeed;
            menuParams.SoftCurrency = _gameplayState.SoftCurrency.CurrentValue;
            var exitParams = new GameplayExitParams(menuParams);
            exitParams.SaveGameplay = true;
            _exitSceneRequest.OnNext(exitParams);
        }
    }
}