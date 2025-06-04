using DI;

namespace MVVM.UI
{
    public abstract class UIManager
    {
        public readonly DIContainer Container;

        protected UIManager(DIContainer container)
        {
            Container = container;
        }
    }
}