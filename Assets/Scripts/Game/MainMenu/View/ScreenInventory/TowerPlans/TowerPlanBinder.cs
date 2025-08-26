using System;
using Game.Common;
using Game.GameRoot.ImageManager;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenInventory.TowerPlans
{
    public class TowerPlanBinder : MonoBehaviour
    {
        private TowerPlanViewModel _viewModel;
        private IDisposable _disposable;

        [SerializeField] private Image towerImage;
        [SerializeField] private TMP_Text textAmount;
        [SerializeField] private Button buttonPopup;
        public void Bind(TowerPlanViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();

            _viewModel = viewModel;
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            
            towerImage.sprite = imageManager.GetTowerPlan(viewModel.ConfigId);
            viewModel.Amount
                .Subscribe(newValue => { textAmount.text = $"x{newValue}"; })
                .AddTo(ref d);
            
            _disposable = d.Build();
            
        }
        
        private void OnEnable()
        {
            buttonPopup.onClick.AddListener(OnOpenPopup);
        }

        private void OnDisable()
        {
            buttonPopup.onClick.RemoveListener(OnOpenPopup);
        }
        
        public void OnOpenPopup()
        {
            _viewModel.RequestOpenPopupTowerPlan();
        }
        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}