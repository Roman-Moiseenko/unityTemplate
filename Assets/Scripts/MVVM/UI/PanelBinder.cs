using UnityEngine;

namespace MVVM.UI
{
    public abstract class PanelBinder<T> : WindowBinder<T> where T : WindowViewModel
    {
        [SerializeField] protected bool isShow;
        [SerializeField] protected RectTransform panel;
        
        public override void Show()
        {
            base.Show();
            isShow = true;
        }
        
        public override void Hide()
        {
            base.Hide();
            isShow = false;
        }

    
    }
}