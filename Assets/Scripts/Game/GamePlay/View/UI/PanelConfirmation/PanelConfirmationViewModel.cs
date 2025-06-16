using DI;
using Game.GamePlay.Commands.RewardCommand;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.State;
using Game.State.Root;
using MVVM.CMD;
using MVVM.UI;
using UnityEngine;

namespace Game.GamePlay.View.UI.PanelConfirmation
{
    public class PanelConfirmationViewModel : WindowViewModel
    {
        public override string Id => "PanelConfirmation";
        public override string Path => "Gameplay/";
        
      //  public readonly int CurrentSpeed;
        public readonly GameplayUIManager _uiManager;
        private readonly DIContainer _container;

        private readonly GameplayStateProxy _gameplayStateProxy;
        
        //Для теста
        private readonly FsmGameplay _fsmGameplay;
        public PanelConfirmationViewModel(
            GameplayUIManager uiManager, 
            DIContainer container
        )
        {
            _uiManager = uiManager;
            _container = container;

            _gameplayStateProxy = container.Resolve<IGameStateProvider>().GameplayState;
            _fsmGameplay = container.Resolve<FsmGameplay>();
        }

        public void RequestConfirmation()
        {
            //TODO Передать текущие координаты и направление
        ///    _fsmGameplay.Fsm.SetState<FsmStateBuildEnd>();
            
            var card = ((FsmStateBuild)_fsmGameplay.Fsm.StateCurrent.Value).GetRewardCard();
            card.Direction = 2;
            card.Position = new Vector2Int(Random.Range(0, 5), Random.Range(0, 2));
            _fsmGameplay.Fsm.SetState<FsmStateBuildEnd>(card);
            
        }
        
        public void RequestCancel()
        {
            //TODO Передать текущие координаты и направление
            _fsmGameplay.Fsm.SetState<FsmStateBuildBegin>();
        }

        public void RequestRotate()
        {
            //TODO Поворот объекта
        }
        
    }
}