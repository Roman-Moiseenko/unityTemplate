using UnityEngine;
using UnityEngine.Serialization;
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
        
        protected virtual void Start()
        {
            btnToggle.onClick.AddListener(OnToggle);
        }

        protected virtual void OnDestroy()
        {
            btnToggle.onClick.RemoveListener(OnToggle);
        }
        protected void Update()
        {
            if (Input.GetMouseButtonDown(0)) {
                if ((Input.mousePosition.x < Point1.x || Input.mousePosition.x > Point2.x) || 
                    (Input.mousePosition.y < Point1.y || Input.mousePosition.y > Point2.y)
                   )
                {
                    container.gameObject.SetActive(false);
                }
            }
        }

        public abstract void OpenDropdown();
        public abstract void CloseDropdown();
        protected virtual void OnToggle()
        {
            if (container.gameObject.activeSelf)
            {
                CloseDropdown();
            }
            else
            {
                OpenDropdown();
            }
        }

        protected abstract void OnBind(T viewModel);
    }
}