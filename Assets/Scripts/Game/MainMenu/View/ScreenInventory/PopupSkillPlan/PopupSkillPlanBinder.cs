using Game.Common;
using Game.GameRoot.ImageManager;
using MVVM.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenInventory.PopupSkillPlan
{
    public class PopupSkillPlanBinder: PopupBinder<PopupSkillPlanViewModel>
    {
        [SerializeField] private Image skillImage;
        [SerializeField] private TMP_Text textCount;
        [SerializeField] private TMP_Text textTitle;
        [SerializeField] private TMP_Text textUse;
        
        protected override void OnBind(PopupSkillPlanViewModel viewModel)
        {
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            base.OnBind(viewModel);
            var skillSettings = viewModel.ViewModel.SkillSettings;
            
            skillImage.sprite = imageManager.GetSkillPlan(viewModel.ViewModel.ConfigId);

            textCount.text = $"x{viewModel.ViewModel.Amount.CurrentValue}";
            textTitle.text = $" ЧЕРТЕЖ {skillSettings.TitleLid}";
            textUse.text = $" * Улучшает:  {skillSettings.TitleLid}";
        }
    }
}