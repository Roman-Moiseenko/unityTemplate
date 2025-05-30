using System;
using R3;

namespace MVVM.UI
{
    public abstract class WindowViewModel : IDisposable
    {
        public Observable<WindowViewModel> CloseRequested => _closeRequested;
        public abstract string Id { get; }

        private readonly Subject<WindowViewModel> _closeRequested = new();

        /**
         * Отправляем запрос на закрыте, которе будет в менеджере (Binder)
         */
        public void RequestClose()
        {
            _closeRequested.OnNext(this);
        }

        public virtual void Dispose() {}


    }
}