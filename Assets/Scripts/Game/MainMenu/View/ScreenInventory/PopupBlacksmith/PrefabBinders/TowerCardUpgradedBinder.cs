using System;
using Game.Common;
using Game.GameRoot.ImageManager;
using Game.MainMenu.View.ScreenInventory.TowerCards;
using Game.State.Inventory;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenInventory.PopupBlacksmith.PrefabBinders
{
    public class TowerCardUpgradedBinder : MonoBehaviour
    {
      //  private TowerCardViewModel _viewModel;
     //   private IDisposable _disposable;

        [SerializeField] private Image epicImage;
        [SerializeField] private Image towerImage;
        [SerializeField] private TMP_Text levelText;


        //TODO Отслеживать и перемещать модель в Binder 
        public ReactiveProperty<bool> IsInDeck = new(false);

        public void Bind(TowerCardUpgradingViewModel viewModel)
        {
         //   var d = Disposable.CreateBuilder();
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();

            towerImage.sprite = imageManager.GetTowerCard(viewModel.ConfigId, 1);
            //  _viewModel = viewModel;
            epicImage.sprite = imageManager.GetEpicLevel(viewModel.EpicLevel.Next());
            
            levelText.text = $"Ур. {viewModel.Level}";


            //     transform.GetComponent<RectTransform>().anchoredPosition = viewModel.Position;
        }

        public void SetLevel(int level)
        {
            levelText.text = $"Ур. {level}";
        }
    }
}