using Game.Common;
using Game.GameRoot.ImageManager;
using Game.State.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameRoot.View.ResourceReward
{
    public class ResourceRewardBinder : MonoBehaviour
    {
        [SerializeField] private Image imageBack;
        [SerializeField] private Image imageCard;
        [SerializeField] private TMP_Text textAmount;
        
        public void Bind(ResourceRewardViewModel viewModel, int square = 170)
        {
            //gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(square, square);
            
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();

            //Debug.Log(viewModel.InventoryType + "  " + viewModel.ConfigId);
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
            
            if (viewModel.InventoryType == InventoryType.SoftCurrency)
            {
                imageBack.sprite = imageManager.GetOther("InventoryBackOther");
                imageCard.sprite = imageManager.GetOther("Currency");
            }
            if (viewModel.InventoryType == InventoryType.HardCurrency)
            {
                imageBack.sprite = imageManager.GetOther("InventoryBackOther");
                imageCard.sprite = imageManager.GetOther("Crystal");
            }
            textAmount.text = $"x{MyFunc.CurrencyToStr(viewModel.Amount)}";

        }
    }
}