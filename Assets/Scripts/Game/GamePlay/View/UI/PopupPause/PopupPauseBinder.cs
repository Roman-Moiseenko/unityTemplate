using MVVM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PopupPause
{
    public class PopupPauseBinder : PopupBinder<PopupPauseViewModal>
    {
        [SerializeField] private Button _btnGoToMenu;
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
        }

        private void OnGoToMenuButtonClicked()
        {
            ViewModel.RequestGoToMainMenu();
        }

        private void OnDisable()
        {
            _btnGoToMenu.onClick.RemoveListener(OnGoToMenuButtonClicked);
        }
        protected override void OnCloseButtonClick()
        {
            //
            ViewModel.RequestClose();
        }
    }
}