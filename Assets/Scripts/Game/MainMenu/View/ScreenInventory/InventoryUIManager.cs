using System;
using DI;
using Game.MainMenu.View.ScreenInventory.PopupBlacksmith;
using Game.MainMenu.View.ScreenInventory.PopupSkillCard;
using Game.MainMenu.View.ScreenInventory.PopupSkillPlan;
using Game.MainMenu.View.ScreenInventory.PopupTowerCard;
using Game.MainMenu.View.ScreenInventory.PopupTowerPlan;
using Game.MainMenu.View.ScreenInventory.SkillCards;
using Game.MainMenu.View.ScreenInventory.SkillPlans;
using Game.MainMenu.View.ScreenInventory.TowerCards;
using Game.MainMenu.View.ScreenInventory.TowerPlans;
using MVVM.UI;
using R3;
using UnityEngine;

namespace Game.MainMenu.View.ScreenInventory
{
    public class InventoryUIManager : UIManager
    {
        /**
         * Отлавливаем события и подписываемся для вызова окна о карточке
         */
        public InventoryUIManager(DIContainer container) : base(container)
        {
            //Карточки башен
            var subjectTowerCard = Container.Resolve<Subject<TowerCardViewModel>>();
            var subjectTowerPlan = Container.Resolve<Subject<TowerPlanViewModel>>();
            subjectTowerCard
                .Subscribe(e => OpenPopupTowerCard(e))
                .AddTo(ref _disposables);
            subjectTowerPlan
                .Subscribe(e => OpenPopupTowerPlan(e))
                .AddTo(ref _disposables);
            
            //Карточки навыков
            var subjectSkillCard = Container.Resolve<Subject<SkillCardViewModel>>();
            var subjectSkillPlan = Container.Resolve<Subject<SkillPlanViewModel>>();
            subjectSkillCard
                .Subscribe(e => OpenPopupSkillCard(e))
                .AddTo(ref _disposables);
            subjectSkillPlan
                .Subscribe(e => OpenPopupSkillPlan(e))
                .AddTo(ref _disposables);
            
            
            //TODO Добавить Героя, Инвентарь
        }
        
        
        private PopupTowerCardViewModel OpenPopupTowerCard(TowerCardViewModel viewModel)
        {
            var b = new PopupTowerCardViewModel(viewModel, Container);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            
            b.CloseRequested.Subscribe(e =>
            {
                //_fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
            }).AddTo(ref _disposables);
            rootUI.OpenPopup(b);
            return b;
        }
        private PopupTowerPlanViewModel OpenPopupTowerPlan(TowerPlanViewModel viewModel)
        {
            var b = new PopupTowerPlanViewModel(viewModel, Container);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            
            b.CloseRequested.Subscribe(e =>
            {
                //_fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
            }).AddTo(ref _disposables);
            rootUI.OpenPopup(b);
            return b;
        }
        
        
        private PopupSkillCardViewModel OpenPopupSkillCard(SkillCardViewModel viewModel)
        {
            var b = new PopupSkillCardViewModel(viewModel, Container);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            
            b.CloseRequested.Subscribe(e =>
            { }).AddTo(ref _disposables);
            rootUI.OpenPopup(b);
            return b;
        }
        private PopupSkillPlanViewModel OpenPopupSkillPlan(SkillPlanViewModel viewModel)
        {
            var b = new PopupSkillPlanViewModel(viewModel, Container);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            
            b.CloseRequested.Subscribe(e =>
            { }).AddTo(ref _disposables);
            rootUI.OpenPopup(b);
            return b;
        }

        /**
         * Кузница башен 
         */
        public PopupBlacksmithTowerViewModel OpenPopupBlacksmithTower()
        {
            var b = new PopupBlacksmithTowerViewModel(Container);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            
            b.CloseRequested.Subscribe(e =>
            {
            }).AddTo(ref _disposables);
            rootUI.OpenPopup(b);
            return b;
            
        }
    }
}