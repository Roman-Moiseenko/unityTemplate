using System;
using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using MVVM.FSM;
using MVVM.UI;

namespace Game.GamePlay.View.UI.PanelBuild
{
    public class PanelBuildViewModel : WindowViewModel
    {
        public override string Id => "PanelBuild";
        public override string Path => "Gameplay/";

        private Object _buttonEntity1;
        private Object _buttonEntity2;
        private Object _buttonEntity3;
        
        private FsmGameplay _fsmGameplay;
        public PanelBuildViewModel(DIContainer container)
        {
            _fsmGameplay = container.Resolve<FsmGameplay>();
        }

        public void GenerateButton()
        {
            //TODO Сервис, который слушает состояние FsmStateBuildBegin
            //Генерирует 3 карточки
            //Здесь присваиваем сущности Entity или бафы Bust
        }
        
        public void OnBuild1()
        {
            //Проверяем тип карточки, если Entity:
            _fsmGameplay.Fsm.SetState<FsmStateBuild>();
            
            //Проверяем тип карточки, если Upgrade:
            //_fsmGameplay.Fsm.SetState<FsmStateBuildEnd>();
        }
        
        public void OnBuild2()
        {
            //Проверяем тип карточки, если Entity:
            _fsmGameplay.Fsm.SetState<FsmStateBuildEnd>();
            
            //Проверяем тип карточки, если Upgrade:
            //_fsmGameplay.Fsm.SetState<FsmStateBuildEnd>();
        }
        public void OnBuild3()
        {
            //Проверяем тип карточки, если Entity:
            _fsmGameplay.Fsm.SetState<FsmStateBuild>();
            
            //Проверяем тип карточки, если Upgrade:
            //_fsmGameplay.Fsm.SetState<FsmStateBuildEnd>();
        }
        
        
    }
}