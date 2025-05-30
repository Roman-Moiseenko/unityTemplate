using DI;

namespace MVVM.UI
{
    public abstract class UIManager
    {
        protected readonly DIContainer Container;

        protected UIManager(DIContainer container)
        {
            Container = container;
        }
    }
}