using DI;

namespace MVVM.UI
{
    public abstract class PanelViewModel : WindowViewModel
    {
        public bool IsShow;
        
        protected PanelViewModel(DIContainer container) : base(container)
        {
        }
    }
}