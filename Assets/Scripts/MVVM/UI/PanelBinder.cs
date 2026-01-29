using UnityEngine;

namespace MVVM.UI
{
    public abstract class PanelBinder<T> : WindowBinder<T> where T : PanelViewModel
    {
        //[SerializeField] protected bool isShow;
        [SerializeField] protected RectTransform panel;
        
        public override void Show()
        {
            base.Show();
            ViewModel.IsShow = true;
        }
        
        public override void Hide()
        {
            base.Hide();
            ViewModel.IsShow = false;
        }

        public bool IsShow()
        {
            return ViewModel.IsShow;
        }

    
    }
}