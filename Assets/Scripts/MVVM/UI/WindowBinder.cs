using UnityEngine;

namespace MVVM.UI
{
    /**
     * Дженерик, для того, чтоб Байндер был привязан к своему классу <T> view-модели
     */
    public abstract class WindowBinder<T> : MonoBehaviour, IWindowBinder where T : WindowViewModel
    {
        protected T ViewModel;

        public void Bind(WindowViewModel viewModel)
        {
            ViewModel = (T)viewModel; //Кешируем view-модель
            OnBind(ViewModel); 
        }

        public virtual void Close()
        {
            //Здесь в дальнейшем будет анимация на закрытие, пока просто уничтоажем окна
            Destroy(gameObject);
        }

        public virtual void Show()
        {
        }

        public virtual void Hide()
        {
        }

        protected virtual void OnBind(T viewModel) { }
    }
}