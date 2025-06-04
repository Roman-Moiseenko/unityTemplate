using DI;
using Game.Settings;
using Game.State;
using Game.State.Root;
using MVVM.UI;
using R3;

namespace Game.GamePlay.View.UI.ScreenGameplay
{
    public class ScreenGameplayViewModel : WindowViewModel
    {
        public readonly GameplayUIManager _uiManager;
        private readonly Subject<Unit> _exitSceneRequest;
        private readonly GameplayState _gameplayState;
        public override string Id => "ScreenGameplay";
        public override string Path => "Gameplay/";
        public ScreenGameplayViewModel(
            GameplayUIManager uiManager, 
            Subject<Unit> exitSceneRequest,
            DIContainer container
            )
        {
            _uiManager = uiManager;
            _exitSceneRequest = exitSceneRequest;
            _gameplayState = container.Resolve<IGameStateProvider>().GameState.GameplayState;
        }
        
        public void RequestOpenPopupA()
        {
            _uiManager.OpenPopupA();
        }
        
        public void RequestOpenPopupB()
        {
            _uiManager.OpenPopupB();
        }

        public void RequestGoToMainMenu()
        {
            _exitSceneRequest.OnNext(Unit.Default); //Вызываем сигнал для смены сцены
        }

        public int RequestGameSpeed()
        {
            var currentGameSpeed = _gameplayState.GetCurrentSpeed();
            var newSpeed = 1;
            switch (currentGameSpeed)
            {
                case 1: newSpeed = 2;
                    break;
                case 2: newSpeed = 4;
                    break;
                case 4: newSpeed = 1;
                    break;
            }
          
            _gameplayState.SetGameSpeed(newSpeed);
            return newSpeed;

        }

    }
}