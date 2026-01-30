using UnityEngine;

namespace MVVM.UI
{
    public abstract class PanelBinder<T> : WindowBinder<T>, IPanelBinder where T : PanelViewModel
    {
        //[SerializeField] protected bool isShow;
        [SerializeField] protected RectTransform panel;
        
      /*  public bool IsShow()
        {
            return ViewModel.IsShow;
        }
*/

        public virtual void Show()
        {
        }

        public virtual void Hide()
        {
            
        }
    }
}