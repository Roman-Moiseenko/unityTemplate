using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.GamePlay.Services;
using Game.Settings;
using Game.State;
using Game.State.GameResources;
using Game.State.Root;
using MVVM.UI;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.UI.ScreenGameplay
{
    public class ScreenGameplayViewModel : WindowViewModel
    {
        public readonly GameplayUIManager _uiManager;
        private readonly Subject<Unit> _exitSceneRequest;
        private readonly GameStateProxy _gameplayState;
        
        
        //TODO Данные для Binder, возможно заменить в дальнейшем прогрузкой анимации и др.
        public readonly ReactiveProperty<int> ProgressData = new();
        public readonly ReactiveProperty<int> SoftCurrency = new();
        public readonly ReactiveProperty<int> HardCurrency = new();
        
        
        
        
   
        public override string Id => "ScreenGameplay";
        public override string Path => "Gameplay/";
     //   public readonly int CurrentSpeed;
        public ScreenGameplayViewModel(
            GameplayUIManager uiManager, 
            Subject<Unit> exitSceneRequest,
            DIContainer container
            )
        {
            _uiManager = uiManager;
            _exitSceneRequest = exitSceneRequest;
            
            _gameplayState = container.Resolve<IGameStateProvider>().GameState;
            _gameplayState.GameplayStateProxy.Progress.Subscribe(newValue => ProgressData.Value = newValue);
 
            var resourcesService = container.Resolve<ResourcesService>();
            resourcesService.ObservableResource(ResourceType.SoftCurrency)
                .Subscribe(newValue => SoftCurrency.Value = newValue);

            resourcesService.ObservableResource(ResourceType.HardCurrency)
                .Subscribe(newValue => HardCurrency.Value = newValue);
 
        }
        
        public void RequestOpenPopupPause()
        {
            _uiManager.OpenPopupPause();
        }
        
        public void RequestOpenPopupB()
        {
            _uiManager.OpenPopupB();
        }

        public void RequestGoToMainMenu()
        {
            _exitSceneRequest.OnNext(Unit.Default); //Вызываем сигнал для смены сцены
        }


    }
}