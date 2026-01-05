using DI;
using Game.Common;
using Game.MainMenu.Root;
using Game.MainMenu.Services;
using Game.MainMenu.View.MainScreen;
using Game.MainMenu.View.ScreenInventory;
using Game.MainMenu.View.ScreenInventory.PopupTowerCard;
using Game.MainMenu.View.ScreenInventory.TowerCards;
using Game.MainMenu.View.ScreenPlay;
using Game.MainMenu.View.ScreenPlay.PopupProfile;
using Game.MainMenu.View.ScreenResearch;
using Game.MainMenu.View.ScreenShop;
using MVVM.UI;
using R3;
using UnityEngine;

namespace Game.MainMenu.View
{
    public class MainMenuUIManager : UIManager
    {
       // private readonly DIContainer _container;

        //   private readonly Subject<Unit> _exitSceneRequestDefault;
        private readonly Subject<MainMenuExitParams> _exitSceneRequest;
        private readonly MainMenuExitParamsService _exitParamsService;
        private readonly UIMainMenuRootViewModel _rootUI;

       // public Vector3 ScaleUI;
        public MainMenuUIManager(DIContainer container) : base(container)
        {
          //  _container = container;
            //       _exitSceneRequestDefault = container.Resolve<Subject<Unit>>(AppConstants.EXIT_SCENE_REQUEST_TAG);
            _exitSceneRequest = container.Resolve<Subject<MainMenuExitParams>>();
            _exitParamsService = container.Resolve<MainMenuExitParamsService>();
            _rootUI = Container.Resolve<UIMainMenuRootViewModel>();
           // ScaleUI = _rootUI.ScaleUI;
        }
/*
        public MainScreenViewModel OpenMainScreen()
        {
            var viewModel = new MainScreenViewModel(this, Container);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            rootUI.OpenScreen(viewModel);
            return viewModel;
        }
        */
        public ScreenShopViewModel OpenScreenShop()
        {
            var viewModel = new ScreenShopViewModel(this, Container);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            _rootUI.OpenScreen(viewModel);
            return viewModel;
        }

        public ScreenInventoryViewModel OpenScreenInventory()
        {
            //TODO Вытаскиваем 
            var viewModel = new ScreenInventoryViewModel(this, Container);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            _rootUI.OpenScreen(viewModel);
            return viewModel;
        }
        
        public ScreenClanViewModel OpenScreenClan()
        {
            var viewModel = new ScreenClanViewModel(this, Container);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            _rootUI.OpenScreen(viewModel);
            return viewModel;
        }
        public ScreenPlayViewModel OpenScreenPlay()
        {
            var viewModel = new ScreenPlayViewModel(this, _exitSceneRequest, _exitParamsService, Container);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            _rootUI.OpenScreen(viewModel);
            return viewModel;
        }
        public ScreenResearchViewModel OpenScreenResearch()
        {
            var viewModel = new ScreenResearchViewModel(this, Container);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            _rootUI.OpenScreen(viewModel);
            return viewModel;
        }

        
        public PopupTowerCardViewModel OpenPopupTowerCard(TowerCardViewModel viewModel)
        {
            var b = new PopupTowerCardViewModel(viewModel, Container);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            b.CloseRequested.Subscribe(e =>
            {
                //_fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
            });
            _rootUI.OpenPopup(b);
            return b;
        }
        
        

        public void OpenPopupResumeGame()
        {
            //TODO
            Debug.Log("Открыть окно возврата в игру");
        }
        
        public PopupProfileViewModel OpenPopupProfile()
        {
            var b = new PopupProfileViewModel(Container);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            
            b.CloseRequested.Subscribe(e =>
            {
            });
            _rootUI.OpenPopup(b);
            return b;
        }
    }
}