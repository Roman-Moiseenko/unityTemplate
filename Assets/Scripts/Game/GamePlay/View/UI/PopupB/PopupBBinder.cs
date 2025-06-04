using Game.GamePlay.View.UI.PopupB;
using MVVM.UI;
using UnityEngine;

namespace Game.GamePlay.View.UI.PopupB
{
    /**
     * Окно выхода из игры
     */
    public class PopupBBinder : PopupBinder<PopupBViewModal>
    {
        //Переписываем OnBind(), когда надо реализовать свое

        protected override void OnBind(PopupBViewModal viewModal)
        {
            base.OnBind(viewModal);
        }

        public override void Close()
        {
            base.Close();
        }
        
    }
}