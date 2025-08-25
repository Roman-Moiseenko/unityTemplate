using System.Collections.Generic;
using System.Globalization;
using Game.Common;
using Game.GameRoot.ImageManager;
using Game.State.Inventory;
using Game.State.Inventory.TowerCards;
using Game.State.Maps.Towers;
using MVVM.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenInventory.PopupTowerCard
{
    public class PopupTowerCardBinder : PopupBinder<PopupTowerCardViewModel>
    {

        [SerializeField] private TMP_Text textTitle;
        [SerializeField] private TMP_Text textDeck;
        [SerializeField] private TMP_Text textUpgrade;
        [SerializeField] private TMP_Text textCost;
        
        [SerializeField] private TMP_Text textDescription;

        [SerializeField] private Image epicImage;
        [SerializeField] private Image towerImage;
        [SerializeField] private TMP_Text textLevel;

        [SerializeField] private List<Transform> parameters = new(6);
        
        protected override void OnBind(PopupTowerCardViewModel viewModel)
        {
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            base.OnBind(viewModel);
            
            parameters.ForEach(item => item.gameObject.SetActive(false));
            
            var towerCardViewModel = viewModel.CardViewModel;
            var towerSettings = viewModel.CardViewModel.TowerSettings;
            
            var towerCardData = (TowerCardData)towerCardViewModel.TowerCard.Origin;
            textTitle.text = $"{towerSettings.TitleLid} ({towerCardViewModel.EpicLevel.CurrentValue.GetString()})";
            textDescription.text = towerSettings.DescriptionLid;
            epicImage.sprite = imageManager.GetEpicLevel(towerCardViewModel.EpicLevel.CurrentValue);
            towerImage.sprite = imageManager.GetTowerCard(towerCardViewModel.ConfigId, 1);

            textLevel.text = $"Ур.: {towerCardData.Level}/{towerCardViewModel.TowerCard.MaxLevel()}";
            textCost.text = towerCardViewModel.TowerCard.GetCostCurrencyLevelUpTowerCard().ToString();

            var index = 0;
            foreach (var parameter in towerCardViewModel.TowerCard.Parameters)
            {
                parameters[index].Find("LeftBlock/Image").GetComponent<Image>().sprite = imageManager.GetParameter(parameter.Key);
                parameters[index].Find("RightBlock/textNameParameter").GetComponent<TMP_Text>().text =
                    parameter.Key.GetString();

                var value = (int)(parameter.Value.Value.CurrentValue * 100) / 100f;
                parameters[index].Find("RightBlock/ValueBlock/Container/textValueParameter").GetComponent<TMP_Text>().text =
                    value.ToString(CultureInfo.CurrentCulture) + " " + parameter.Key.GetMeasure();
                parameters[index].gameObject.SetActive(true);
                index++;
            }
            
        }
        
    }
}