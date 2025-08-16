using System;
using R3;

namespace MVVM.UI
{
    public abstract class WindowViewModel : IDisposable
    {
        public Observable<WindowViewModel> CloseRequested => _closeRequested; //На событие закрытия можно подписаться извне
        public abstract string Id { get; }
        public abstract string Path { get; }

        private readonly Subject<WindowViewModel> _closeRequested = new(); //Создаем событие для закрытия окна

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