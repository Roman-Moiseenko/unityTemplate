using System;
using Game.Common;
using Game.GameRoot.ImageManager;
using Game.MainMenu.View.ScreenInventory.TowerCards;
using Game.State.Inventory;
using Game.State.Inventory.TowerCards;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenInventory.PopupBlacksmith.PrefabBinders
{
    public class TowerCardResourceBinder : MonoBehaviour
    {
        [SerializeField] private Image epicImage;
        [SerializeField] private Image towerImage;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private Button buttonSet;
        [SerializeField] private Image selectImage;
        
        private TowerCardResourceViewModel _viewModel;
        private IDisposable _disposable;
        public string ConfigId => _viewModel.ConfigId;
        public TypeEpicCard EpicLevel => _viewModel.EpicLevel;

        public void Bind(TowerCardResourceViewModel viewModel)
        {
            _viewModel = viewModel;
            var d = Disposable.CreateBuilder();
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();

            towerImage.sprite = imageManager.GetTowerCard(viewModel.ConfigId, 1);
            epicImage.sprite = imageManager.GetEpicLevel(viewModel.EpicLevel);
            levelText.text = $"Ур. {viewModel.Level}";
            _viewModel.IsSelected.Subscribe(v => selectImage.gameObject.SetActive(v));
            
            _disposable = d.Build();
        }
        
        private void OnEnable()
        {
            buttonSet.onClick.AddListener(OnSelectedCard);
        }

        private void OnDisable()
        {
            buttonSet.onClick.RemoveListener(OnSelectedCard);
        }

        public void OnSelectedCard()
        {
            //TODO Включить анимацию
            _viewModel.Position = transform.position;
            _viewModel.RequestSelectedTowerCard();
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}