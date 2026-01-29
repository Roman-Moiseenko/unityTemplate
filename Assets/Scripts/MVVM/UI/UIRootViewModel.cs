using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using ObservableCollections;
using R3;
using Scripts.Game.GameRoot;
using UnityEngine;

namespace MVVM.UI
{
    /**
     * Контейнер, в котором будут открыты popup и окно
     */
    public class UIRootViewModel : IDisposable
    {
        public DIContainer Container { get; }

        //Публичные поля, для подписки и чтения 
        public IObservableCollection<WindowViewModel> OpenedPopups => _openedPopups;
        public IObservableCollection<PanelViewModel> OpenedPanels => _openedPanels;
        public ReadOnlyReactiveProperty<WindowViewModel> OpenedScreen => _openedScreen;
        
        //Приватные поля для изменения
        private readonly ReactiveProperty<WindowViewModel> _openedScreen = new(null);
        private readonly ObservableList<WindowViewModel> _openedPopups = new(); //Массив открытых окон
        private readonly ObservableList<PanelViewModel> _openedPanels = new(); //Массив открытых панелей
        private readonly Dictionary<WindowViewModel, IDisposable> _popupSubscriptions = new ();
        
        public ReactiveProperty<WindowViewModel> ShowedPanel = new();
        public ReactiveProperty<WindowViewModel> HidedPanel = new();

        public ReactiveProperty<bool> CloseAllPopupHandler = new(false);
        public ReactiveProperty<bool> HideAllPanelHandler = new(false);

        public Vector3 ScaleUI;
        
        public UIRootViewModel(DIContainer container)
        {
            Container = container;
            //var uiRoot = container.Resolve<UIRootView>();
            //ScaleUI = uiRoot.GetScale();
        }
        public void Dispose()
        {
            CloseAllPopups();
            _openedScreen.Value?.Dispose();
            DisposeAllPanels();
        }

        public void AddPanel(PanelViewModel panelViewModel)
        {
            if (_openedPanels.Contains(panelViewModel)) return;
            _openedPanels.Add(panelViewModel);
        }

        public void OpenScreen(WindowViewModel screenViewModel)
        {
            _openedScreen.Value?.Dispose(); //Если текущий экран существует/открыт, то закрываем
            screenViewModel.ScaleUI = ScaleUI;
            _openedScreen.Value = screenViewModel;
        }
        
        public void ShowPanel<T>() 
        {
            var type = typeof(T);
            _openedPanels.ForEach(action =>
            {
                if (action.GetType() == type) ShowedPanel.OnNext(action);
            });
        }
        
        public void HidePanel<T>()
        {
            var type = typeof(T);
            _openedPanels.ForEach(action =>
            {
                if (action.GetType() == type) HidedPanel.OnNext(action);
            });
        }
        
        public bool IsOpenedPanel<T>()
        {
            foreach (var panel in _openedPanels)
            {
                if (panel.GetType() == typeof(T) && panel.IsShow)
                    return true;
            }
            return false;
        }
        
        public void OpenPopup(WindowViewModel popupViewModel)
        {
            if (_openedPopups.Contains(popupViewModel)) return;

            //Подписка на закрытие окна
            var subscription = popupViewModel.CloseRequested.Subscribe(ClosePopup);
            _popupSubscriptions.Add(popupViewModel, subscription);
            _openedPopups.Add(popupViewModel);
        }

        public void ClosePopup(WindowViewModel popupViewModel)
        {
            //При закрытии Popup, если оно открыто
            if (_openedPopups.Contains(popupViewModel))
            {
                popupViewModel.Dispose(); //Освобождаем ресурсы окна popup
                _openedPopups.Remove(popupViewModel); //удаляем окно из списка открытых окон
                _popupSubscriptions[popupViewModel]?.Dispose();
                _popupSubscriptions.Remove(popupViewModel); //Удаляем из кеша подписки
            }
        }

        public void ClosePopup(string popupId)
        {
            var openedPopupViewModal = _openedPopups.FirstOrDefault(p => p.Id == popupId);
            ClosePopup(openedPopupViewModal);
        }

        private void CloseAllPopups()
        {
            foreach (var openedPopup in _openedPopups)
            {
                ClosePopup(openedPopup);
            }
        }
        
        private void DisposeAllPanels()
        {
            foreach (var openedPanel in _openedPanels.ToList())
            {
                _openedPanels.Remove(openedPanel);
                openedPanel.Dispose();
            }
        }
    }
}