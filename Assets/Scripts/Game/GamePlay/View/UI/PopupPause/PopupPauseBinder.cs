using MVVM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PopupPause
{
    public class PopupPauseBinder : PopupBinder<PopupPauseViewModal>
    {
        [SerializeField] private Button _btnGoToMenu;
        [SerializeField] private Button _btnExitSave;
        //Поля кнопок во всплывающем окне
        //Например при проигрыше - а) выйти, б) посмотреть рекламу и продолжить, в) купить продолжение за кристаллы
        //Переписываем OnBind(), когда надо реализовать свое
/*
        protected override void OnBind(PopupAViewModal viewModal)
        {
            base.OnBind(viewModal);
        }
        */

        //Подписываемся на нажатия кнопок и вызываем функции из View Модели
        private void OnEnable()
        {
            _btnGoToMenu.onClick.AddListener(OnGoToMenuButtonClicked);
            _btnExitSave.onClick.AddListener(OnExitSaveClicked);
        }

        private void OnDisable()
        {
            _btnGoToMenu.onClick.RemoveListener(OnGoToMenuButtonClicked);
            _btnExitSave.onClick.RemoveListener(OnExitSaveClicked);
        }
        private void OnExitSaveClicked()
        {
            ViewModel.RequestExitSave();
        }

        private void OnGoToMenuButtonClicked()
        {
            ViewModel.RequestGoToMainMenu();
        }

        protected override void OnCloseButtonClick()
        {
            //
            ViewModel.RequestClose();
        }
    }
}