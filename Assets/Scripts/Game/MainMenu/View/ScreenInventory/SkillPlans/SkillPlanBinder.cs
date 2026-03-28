using System;
using Game.Common;
using Game.GameRoot.ImageManager;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenInventory.SkillPlans
{
    public class SkillPlanBinder : MonoBehaviour
    {
        private SkillPlanViewModel _viewModel;
        private IDisposable _disposable;

        [SerializeField] private Image skillImage;
        [SerializeField] private TMP_Text textAmount;
        [SerializeField] private Button buttonPopup;
        public void Bind(SkillPlanViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();

            _viewModel = viewModel;
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            
            skillImage.sprite = imageManager.GetSkillPlan(viewModel.ConfigId);
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
            _viewModel.RequestOpenPopupSkillPlan();
        }
        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}