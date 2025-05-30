using System;
using ObservableCollections;
using R3;
using UnityEngine;

namespace MVVM.UI
{
    public class UIRootBinder : MonoBehaviour
    {
        [SerializeField] private WindowsContainer _windowsContainer;
        private readonly CompositeDisposable _subscriptions = new();
        public void Bind(UIRootViewModel viewModel)
        {
            _subscriptions.Add(viewModel.OpenedScreen.Subscribe(newScreenViewModel =>
            {
                _windowsContainer.OpenScreen(newScreenViewModel);
            }));
            
            foreach (var openedPopup in viewModel.OpenedPopups)
            {
                _windowsContainer.OpenPopup(openedPopup);
            }
            
            _subscriptions.Add(viewModel.OpenedPopups.ObserveAdd().Subscribe(e =>
            {
                var newPopupViewModel = e.Value;
                _windowsContainer.OpenPopup(newPopupViewModel);
            }));
            
            _subscriptions.Add(viewModel.OpenedPopups.ObserveRemove().Subscribe(e =>
            {
                var newPopupViewModel = e.Value;
                _windowsContainer.ClosePopup(newPopupViewModel);
            }));
            OnBind(viewModel);
        }

        protected virtual void OnBind(UIRootViewModel viewModel) { }

        private void OnDestroy()
        {
            _subscriptions.Dispose();
        }
    }
}