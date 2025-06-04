using MVVM.UI;

namespace Game.GamePlay.View.UI.PopupA
{
    public class PopupABinder : PopupBinder<PopupAViewModal>
    {
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
        
        protected override void OnCloseButtonClick()
        {
            //
            ViewModel.RequestClose();
        }
    }
}