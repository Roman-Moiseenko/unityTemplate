using System;
using DI;
using R3;
using Scripts.Game.GameRoot;
using UnityEngine;

namespace MVVM.UI
{
    public abstract class WindowViewModel : IDisposable
    {
        public Observable<WindowViewModel> CloseRequested => _closeRequested; //На событие закрытия можно подписаться извне
        public abstract string Id { get; }
        public abstract string Path { get; }

        public Vector3 ScaleUI;
        public DIContainer Container;
        private readonly Subject<WindowViewModel> _closeRequested = new(); //Создаем событие для закрытия окна

        protected WindowViewModel(DIContainer container)
        {
            Container = container;
            var uiRoot = container.Resolve<UIRootView>();
            ScaleUI = uiRoot.GetScale();
        }

        /**
         * Отправляем запрос на закрыте, которе будет в менеджере (Binder)
         */
        public virtual void RequestClose()
        {
            _closeRequested.OnNext(this);
        }

        public virtual void Dispose() {}


    }
}