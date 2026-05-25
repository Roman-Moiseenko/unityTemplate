using System;
using DI;
using R3;

namespace MVVM.UI
{
    public abstract class UIManager : IDisposable
    {
        public readonly DIContainer Container;
        protected DisposableBag _disposables = new();
        protected UIManager(DIContainer container)
        {
            Container = container;
        }

        public virtual void Dispose()
        {
            _disposables.Dispose();
        }
    }
}