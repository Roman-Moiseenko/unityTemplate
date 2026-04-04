using System;
using DI;
using Game.GamePlay.Commands.RewardCommand;
using Game.GamePlay.Fsm;
using Game.GamePlay.Services;
using Game.GamePlay.View.Skills;
using Game.State;
using Game.State.Gameplay;
using Game.State.Root;
using MVVM.CMD;
using MVVM.UI;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.UI.PanelActions
{
    public class PanelActionsViewModel : PanelViewModel
    {
        public override string Id => "PanelActions";
        public override string Path => "Gameplay/Panels/Actions/";
        
       // public readonly GameplayUIManager _uiManager;
        
        //public readonly ReactiveProperty<int> CurrentSpeed;
        private readonly GameplayStateProxy _gameplayStateProxy;
        private SkillsService _skillService;
        private IDisposable _disposable;

        public SkillViewModel SkillOneViewModel;
        public SkillViewModel SkillTwoViewModel;
        
        public PanelActionsViewModel(
            GameplayUIManager uiManager, 
            DIContainer container
        ) : base(container)
        {
            var d = Disposable.CreateBuilder();
            IsShow = true; //По-умолчанию показываем
            var _uiManager = uiManager;
            _gameplayStateProxy = container.Resolve<IGameStateProvider>().GameplayState;
            var fsmGameplay = container.Resolve<FsmGameplay>();
            fsmGameplay.Fsm.StateCurrent.Subscribe();
            _disposable = d.Build();
            _skillService = container.Resolve<SkillsService>();
            SkillOneViewModel = _skillService.SkillOne;
            SkillTwoViewModel = _skillService.SkillTwo;
        }
        public void RequestGameSpeed()
        {
            _gameplayStateProxy.SetNextSpeed();
        }

        public void RequestToProgressAdd()
        {
            var cmd = Container.Resolve<ICommandProcessor>();
            var command = new CommandRewardKillMob(25, 1);
            cmd.Process(command);
        }
        
        public override void Dispose()
        {
            _disposable.Dispose();
        }

        public float GetCurrentSpeed()
        {
            return _gameplayStateProxy.GetCurrentSpeed();
        }

        public void RequestBuySpeed4x()
        {
            //TODO Запуск процедуры покупки
            Debug.Log("RequestBuySpeed4x");
        }

        public void RequestStartSkillOne()
        {
            
        }

        public void RequestStartSkillTwo()
        {
            
        }
    }
}