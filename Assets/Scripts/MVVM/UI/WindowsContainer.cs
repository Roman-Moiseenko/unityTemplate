using System.Collections.Generic;
using UnityEngine;

namespace MVVM.UI
{
    public class WindowsContainer : MonoBehaviour
    {
        [SerializeField] private Transform _screensContainer; //контейнер экранов
        [SerializeField] private Transform _popupsContainer; //контейнер попап-ов
        [SerializeField] private Transform _panelsContainer; //контейнер попап-ов

        private readonly Dictionary<WindowViewModel, IWindowBinder>
            _openedPanelBinders = new(); //кеш открытых binder-ов для панелей
        private readonly Dictionary<WindowViewModel, IWindowBinder>
            _openedPopupBinders = new(); //кеш открытых binder-ов для попап

        private IWindowBinder _openedScreenBinder; //кеш для байндера открытого окна 

        /**
         * Добавляем панели при загрузке, они всегда в памяти и только показываются или скрываются
         */
        public void AddPanel(WindowViewModel viewModel)
        {
           // Debug.Log("Этап 3. Создание панели " + viewModel.GetType());
            var prefabPath = GetPrefabPath(viewModel);
            var prefab = Resources.Load<GameObject>(prefabPath);
            var createPanel = Instantiate(prefab, _panelsContainer);
            var binder = createPanel.GetComponent<IWindowBinder>();
            binder.Bind(viewModel);
            _openedPanelBinders.Add(viewModel, binder);
        }
        
        public void ShowPanel(WindowViewModel viewModel)
        {
            var binder = _openedPanelBinders[viewModel];
            binder.Show();
        }

        public void HidePanel(WindowViewModel viewModel)
        {
            var binder = _openedPanelBinders[viewModel];
            binder.Hide();
        }
        
        /**
         * Открытие окна попап - всегда создание из префаба, по закрытию - выгрузка из памяти
         */
        public void OpenPopup(WindowViewModel viewModel)
        {
            var prefabPath = GetPrefabPath(viewModel); //Получаем путь к префабу из view-модели
            var prefab = Resources.Load<GameObject>(prefabPath); //Загружаем префаб
            var createPopup = Instantiate(prefab, _popupsContainer); //Создаем префаб в контейнере _popupsContainer
            var binder = createPopup.GetComponent<IWindowBinder>(); //Достаем binder из префаба
            binder.Bind(viewModel);
            _openedPopupBinders.Add(viewModel, binder); //кешируем открый байндер
        }
        
        public void ClosePopup(WindowViewModel popupViewModel)
        {
            var binder = _openedPopupBinders[popupViewModel];
            binder?.Close();
            _openedPopupBinders.Remove(popupViewModel);
        }

        public void OpenScreen(WindowViewModel viewModel)
        {
            if (viewModel == null) return;
            _openedScreenBinder?.Close();
            var prefabPath = GetPrefabPath(viewModel); //Получаем путь к префабу из view-модели
            var prefab = Resources.Load<GameObject>(prefabPath); //Загружаем префаб
            var createScreen = Instantiate(prefab, _screensContainer); //Создаем префаб в контейнере _popupsContainer
            var binder = createScreen.GetComponent<IWindowBinder>(); //Достаем binder из префаба
            
            binder.Bind(viewModel);
            _openedScreenBinder = binder;
        }
        
        private string GetPrefabPath(WindowViewModel viewModel)
        {
            var categoryName = viewModel.Path ?? "";
            return $"Prefabs/UI/{categoryName}{viewModel.Id}";
        }
    }
}