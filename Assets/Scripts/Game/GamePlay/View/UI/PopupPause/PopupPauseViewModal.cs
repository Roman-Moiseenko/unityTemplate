using DI;
using Game.GamePlay.Root;
using Game.GamePlay.Services;
using Game.MainMenu.Root;
using Game.State;
using Game.State.Gameplay;
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
      //  private readonly Subject<GameplayExitParams> _exitSceneRequest;

        public readonly GameplayStateProxy GameplayState;

        private readonly GameplayUIManager _uiManager;
        //TODO действия при нажатии на кнопки

        public PopupPauseViewModal(
            GameplayUIManager uiManager, DIContainer container) : base(container)
        {
            //_exitSceneRequest = exitSceneRequest;
            _uiManager = uiManager;
            GameplayState = container.Resolve<IGameStateProvider>().GameplayState;
        }

        //Проигрыш 
        //Вызываем события закрытия окна с проигришем

        //

        //Вызываем событие просмотр рекламы, после завершения событие восстановления

        //Покупка восстановления ... отнимаем кристаллы, событие восстановления

        public void Win()
        {
            var gs = Container.Resolve<GameplayService>();
            gs.Win();
        }
        
        public void Lose()
        {
            var gs = Container.Resolve<GameplayService>();
            gs.Lose();
        }

        public void RequestToExit()
        {
            _uiManager.OpenPopupExitNotSave();
        }

        public void RequestToSettings()
        {
            _uiManager.OpenPopupSettings();
        }

        public void RequestToStatistic()
        {
            _uiManager.OpenPopupStatistics();
        }
    }
}