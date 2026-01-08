using Game.GamePlay.Classes;
using UnityEngine;
using UnityEngine.UI;

namespace MVVM.UI
{
    public abstract class DropdownBinder<T> : MonoBehaviour where T : DropdownViewModel
    {
        
        [SerializeField] private Button btnToggle;
        [SerializeField] protected Transform container;
        
        protected T ViewModel;
        protected Vector2 Point1;
        protected Vector2 Point2;
        public void Bind(T viewModel)
        {
            ViewModel = viewModel;
            container.gameObject.SetActive(false);
            OnBind(ViewModel);
            var worldCorners = new Vector3[4];
            container.gameObject.GetComponent<RectTransform>().GetWorldCorners(worldCorners);
            Point1 = new Vector2(worldCorners[0].x, worldCorners[0].y);
            Point2 = new Vector2(worldCorners[2].x, worldCorners[2].y);
        }

        private void OnEnable()
        {
            InputManager.OnTapUI += CheckClickOut;
        }

        private void OnDisable()
        {
            InputManager.OnTapUI -= CheckClickOut;
        }

        private void CheckClickOut(Vector2 position)
        {
            if ((position.x < Point1.x || position.x > Point2.x) || 
                (position.y < Point1.y || position.y > Point2.y)
               )
            {
                CloseDropdown();
            }
        }
        protected virtual void Start()
        {
            btnToggle.onClick.AddListener(OpenDropdown);
        }

        protected virtual void OnDestroy()
        {
            btnToggle.onClick.RemoveListener(OpenDropdown);
        }

        protected virtual void OpenDropdown()
        {
            container.gameObject.SetActive(true);
        }

        protected virtual void CloseDropdown()
        {
            container.gameObject.SetActive(false);
        }

        protected abstract void OnBind(T viewModel);
    }
}