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
                imageBack.sprite = imageManager.GetEpicLevel(TypeEpicCard.Normal);
                imageCard.sprite = imageManager.GetTowerCard(viewModel.ConfigId, 1);
            }

            if (viewModel.InventoryType == InventoryType.TowerPlan)
            {
                imageBack.sprite = imageManager.GetTowerPlan("Background");
                imageCard.sprite = imageManager.GetTowerPlan(viewModel.ConfigId);
            }

            if (viewModel.InventoryType == InventoryType.Other)
            {
                imageBack.sprite = imageManager.GetEpicLevel(TypeEpicCard.Normal);
                imageCard.sprite = imageManager.GetOther(viewModel.ConfigId);
            }

            textAmount.text = CurrencyToStr(viewModel.Amount);

        }

        private string CurrencyToStr(long value)
        {
            float t;
            switch (value)
            {
                case >= 1000000000:
                    t = (int)(value / 10000000) / 100f;
                    return $"{t} B";
                case >= 1000000:
                    t = (int)(value / 10000) / 100f;
                    return $"{t} M";
                case >= 1000:
                    t = (int)(value / 10) / 100f;
                    return $"{t} K";
                default:
                    return value.ToString();
            }
        }
    }
}