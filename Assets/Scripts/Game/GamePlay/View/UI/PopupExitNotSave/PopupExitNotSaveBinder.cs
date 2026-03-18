using MVVM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PopupExitNotSave
{
    public class PopupExitNotSaveBinder : PopupBinder<PopupExitNotSaveViewModel>
    {
        [SerializeField] private Button btnExit;
        [SerializeField] private Button btnContinue;
        protected override void OnBind(PopupExitNotSaveViewModel viewModal)
        {
            base.OnBind(viewModal);
        }
        
        private void OnEnable()
        {
            btnExit.onClick.AddListener(OnExitClick);
            btnContinue.onClick.AddListener(OnContinueClick);
        }

        private void OnDisable()
        {
            btnExit.onClick.RemoveListener(OnExitClick);
            btnContinue.onClick.RemoveListener(OnContinueClick);

        }

        private void OnExitClick()
        {
            ViewModel.RequestExit();
        }
        
        private void OnContinueClick()
        {
            ViewModel.RequestContinue();
        }
    }
}