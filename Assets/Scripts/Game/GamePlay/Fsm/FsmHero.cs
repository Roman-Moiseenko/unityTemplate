using System;
using DI;
using Game.GamePlay.Fsm.HeroStates;
using MVVM.FSM;
using R3;

namespace Game.GamePlay.Fsm
{
    public class FsmHero : IDisposable
    {
        public FsmProxy Fsm;
        private DisposableBag _disposables;

        public FsmHero(DIContainer container)
        {
            Fsm = new FsmProxy();
            Fsm.AddState(new FsmHeroAwait(Fsm));
            Fsm.AddState(new FsmHeroMoving(Fsm));
            Fsm.AddState(new FsmHeroPlacement(Fsm));
            Fsm.AddState(new FsmHeroPlacementEnd(Fsm));
            Fsm.AddState(new FsmHeroAttack(Fsm));
            Fsm.AddState(new FsmHeroSelected(Fsm, container));
            Fsm.AddState(new FsmHeroUnSelected(Fsm));
            Fsm.SetState<FsmHeroAwait>();
        }

        public bool IsSelected()
        {
            return Fsm.StateCurrent.Value.GetType() == typeof(FsmHeroSelected);
        }
        
        public void Dispose()
        {
            Fsm?.Dispose();
            _disposables.Dispose();
        }

        public bool IsPlacement()
        {
            return Fsm.StateCurrent.Value.GetType() == typeof(FsmHeroPlacement);
        }
    }
}