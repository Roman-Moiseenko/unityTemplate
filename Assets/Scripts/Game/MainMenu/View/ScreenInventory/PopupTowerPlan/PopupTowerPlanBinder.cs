using Game.Common;
using Game.GameRoot.ImageManager;
using MVVM.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenInventory.PopupTowerPlan
{
    public class PopupTowerPlanBinder : PopupBinder<PopupTowerPlanViewModel>
    {
        [SerializeField] private Image towerImage;
        [SerializeField] private TMP_Text textCount;
        [SerializeField] private TMP_Text textTitle;
        [SerializeField] private TMP_Text textUse;

        protected override void OnBind(PopupTowerPlanViewModel viewModel)
        {
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            base.OnBind(viewModel);
            var towerSettings = viewModel.PlanViewModel.TowerSettings;
            
            towerImage.sprite = imageManager.GetTowerPlan(viewModel.PlanViewModel.ConfigId);

            textCount.text = $"x{viewModel.PlanViewModel.Amount.CurrentValue}";
            textTitle.text = $" ЧЕРТЕЖ {towerSettings.TitleLid}";
            textUse.text = $" * Улучшает:  {towerSettings.TitleLid}";
        }
    }
}