using System;
using Game.Common;
using Game.GameRoot.ImageManager;
using Game.MainMenu.View.ScreenInventory.TowerCards;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenInventory.PopupBlacksmith.PrefabBinders
{
    public class TowerCardUpgradingBinder : MonoBehaviour
    {
        private TowerCardViewModel _viewModel;
        private IDisposable _disposable;

        [SerializeField] private Image epicImage;
        [SerializeField] private Image towerImage;
        [SerializeField] private TMP_Text levelText;
        

        //TODO Отслеживать и перемещать модель в Binder 
        public ReactiveProperty<bool> IsInDeck = new(false);

        public void Bind(TowerCardViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            
            towerImage.sprite = imageManager.GetTowerCard(viewModel.ConfigId, 1);
            _viewModel = viewModel;
            
            viewModel.EpicLevel
                .Subscribe(newValue => epicImage.sprite = imageManager.GetEpicLevel(newValue))
                .AddTo(ref d);
            viewModel.Level
                .Subscribe(newValue => { levelText.text = $"Ур. {newValue}"; })
                .AddTo(ref d);

       //     transform.GetComponent<RectTransform>().anchoredPosition = viewModel.Position;
            _disposable = d.Build();
        }
        
        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}