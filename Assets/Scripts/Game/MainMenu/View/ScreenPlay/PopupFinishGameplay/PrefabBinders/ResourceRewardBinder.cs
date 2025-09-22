using Game.Common;
using Game.GameRoot.ImageManager;
using Game.State.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenPlay.PopupFinishGameplay.PrefabBinders
{
    public class ResourceRewardBinder : MonoBehaviour
    {
        [SerializeField] private Image imageBack;
        [SerializeField] private Image imageCard;
        [SerializeField] private TMP_Text textAmount;
        
        public void Bind(ResourceRewardViewModel viewModel)
        {
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();

            if (viewModel.InventoryType == InventoryType.TowerCard)
            {
                imageBack.sprite = imageManager.GetOther("InventoryBackOther");
                imageCard.sprite = imageManager.GetTowerCard(viewModel.ConfigId, 1);
            }

            if (viewModel.InventoryType == InventoryType.TowerPlan)
            {
                imageBack.sprite = imageManager.GetOther("InventoryBackTower");
                imageCard.sprite = imageManager.GetTowerPlan(viewModel.ConfigId);
            }

            if (viewModel.InventoryType == InventoryType.Other)
            {
                imageBack.sprite = imageManager.GetOther("InventoryBackOther");
                imageCard.sprite = imageManager.GetOther(viewModel.ConfigId);
            }

            textAmount.text = $"x{Func.CurrencyToStr(viewModel.Amount)}";

        }
    }
}