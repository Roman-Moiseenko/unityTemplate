using System;
using Game.Common;
using Game.GameRoot.ImageManager;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenInventory.SkillCards
{
    public class SkillCardBinder : MonoBehaviour
    {
        private SkillCardViewModel _viewModel;
        private IDisposable _disposable;

        [SerializeField] private Image epicImage;
        [SerializeField] private Image skillImage;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private Button buttonPopup;
        [SerializeField] private Transform canIsUpdate;
        
        //TODO Отслеживать и перемещать модель в Binder 
        public ReactiveProperty<bool> IsInDeck = new(false);

        public void Bind(SkillCardViewModel viewModel)
        {
            _viewModel = viewModel;

            var d = Disposable.CreateBuilder();
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            skillImage.sprite = imageManager.GetSkillCard(viewModel.ConfigId);
            _viewModel = viewModel;
            
            viewModel.EpicLevel
                .Subscribe(newValue => epicImage.sprite = imageManager.GetEpicLevel(newValue))
                .AddTo(ref d);
            viewModel.Level
                .Subscribe(newValue => { levelText.text = $"Ур. {newValue}"; })
                .AddTo(ref d);
            viewModel.IsCanUpdate
                .Subscribe(v => canIsUpdate.gameObject.SetActive(v))
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

        private void OnOpenPopup()
        {
            _viewModel.RequestOpenPopupSkillCard();
        }
        
        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}