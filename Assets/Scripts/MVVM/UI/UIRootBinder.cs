using System;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using UnityEngine;

namespace MVVM.UI
{
    public class UIRootBinder : MonoBehaviour
    {
        [SerializeField] protected WindowsContainer _windowsContainer;


        private IDisposable _disposable;

        public void Bind(UIRootViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            viewModel.OpenedScreen
                .Subscribe(newScreenViewModel => _windowsContainer.OpenScreen(newScreenViewModel)).AddTo(ref d);

            foreach (var openedPopup in viewModel.OpenedPopups)
            {
                _windowsContainer.OpenPopup(openedPopup);
            }

            foreach (var openedPanel in viewModel.OpenedPanels)
            {
                _windowsContainer.AddPanel(openedPanel);
            }

            viewModel.ShowedPanel.Subscribe(showViewModel =>
            {
                if (showViewModel != null) _windowsContainer.ShowPanel(showViewModel);
            }).AddTo(ref d);
            viewModel.HidedPanel.Subscribe(hideViewModel =>
            {
                if (hideViewModel != null) _windowsContainer.HidePanel(hideViewModel);
            }).AddTo(ref d);
            
            viewModel.OpenedPanels.ObserveAdd().Subscribe(e => _windowsContainer.AddPanel(e.Value)).AddTo(ref d);

            viewModel.OpenedPopups.ObserveAdd().Subscribe(e =>
            {
                var newPopupViewModel = e.Value;
                _windowsContainer.OpenPopup(newPopupViewModel);
            }).AddTo(ref d);

            viewModel.OpenedPopups.ObserveRemove().Subscribe(e =>
            {
                var newPopupViewModel = e.Value;
                _windowsContainer.ClosePopup(newPopupViewModel);
            }).AddTo(ref d);
            OnBind(viewModel);

            viewModel.CloseAllPopupHandler.Where(x => x).Subscribe(_ => _windowsContainer.CloseAllPopup()).AddTo(ref d);
            viewModel.HideAllPanelHandler.Where(x => x).Subscribe(_ => _windowsContainer.HideAllPanel()).AddTo(ref d);
            
            _disposable = d.Build();
            
            
            ///viewModel.CloseAllPopups();
        }

        protected virtual void OnBind(UIRootViewModel viewModel)
        {
        }

        private void OnDestroy()
        {
           // _disposable?.Dispose();
        }
    }
}