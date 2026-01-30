using MVVM.UI;
using UnityEngine;
using UnityEngine.UI;


namespace Game.GamePlay.View.UI.PopupTowerDelete
{
    public class PopupTowerDeleteBinder : PopupBinder<PopupTowerDeleteViewModel>
    {
        [SerializeField] private Button btnDelete;
        [SerializeField] private Button btnCancel;
        
        protected override void OnBind(PopupTowerDeleteViewModel viewModel)
        {
            base.OnBind(viewModel);
            
        }
        
        private void OnEnable()
        {
            btnDelete.onClick.AddListener(OnTowerDelete);
            btnCancel.onClick.AddListener(OnCancel);
        }

        private void OnDisable()
        {
            btnDelete.onClick.RemoveListener(OnTowerDelete);
            btnCancel.onClick.RemoveListener(OnCancel);
        }
        private void OnTowerDelete()
        {
            ViewModel.RequestDelete();
        }

        private void OnCancel()
        {
            ViewModel.RequestCancel();
        }

        protected override void OnCloseButtonClick()
        {
            ViewModel.RequestClose();
        }
    }
}