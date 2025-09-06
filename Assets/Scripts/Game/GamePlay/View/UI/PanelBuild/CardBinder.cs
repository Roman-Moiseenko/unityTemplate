using System;
using Game.Common;
using Game.GameRoot.ImageManager;
using Game.Settings;
using Game.State.Gameplay;
using Game.State.Maps.Towers;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelBuild
{
    public class CardBinder : MonoBehaviour
    {
        private IDisposable _disposable;
        private ImageManagerBinder _imageManager;
        
        //BUTTONS
        [SerializeField] private Button buttonBuild;
        [SerializeField] private Button buttonShowInfo;
        [SerializeField] private Button buttonHideInfo;
        
        //FRONTEND
        [SerializeField] private TMP_Text textCaption;
        [SerializeField] private Image imageDefence;
        [SerializeField] private TMP_Text textDescription;
        [SerializeField] private Image imageCard;
        [SerializeField] private Image imageBack;

        //BACKEND
        [SerializeField] private TMP_Text textDescriptionBack;
        [SerializeField] private Image imageBackBack;
        //Изображение башни на обороте
        [SerializeField] private Image imageCardTowerBack;
        //Изображение остального на обороте
        [SerializeField] private Image imageCardOtherBack;
        [SerializeField] private Transform blockParameters;
        


        //  private GameSettings _gameSettings;
        private CardViewModel _viewModel;
        private Animator _animator;

        private void Awake()
        {
            _imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
        }

        public void Bind(CardViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _animator = gameObject.GetComponent<Animator>();
            _viewModel = viewModel;
            _viewModel.Updated.Subscribe(_ =>
            {
                textCaption.text = _viewModel.Caption;
                textDescription.text = _viewModel.Description;
                if (_viewModel.Defence != null)
                {
                    imageDefence.sprite = _imageManager.GetDefence(_viewModel.Defence);
                    imageDefence.gameObject.SetActive(true);
                }
                else
                {
                    imageDefence.gameObject.SetActive(false);
                }

                imageBack.sprite = _viewModel.RewardType switch
                {
                    RewardType.Tower => _imageManager.GetEpicLevel(_viewModel.ImageBack),
                    _ => _imageManager.GetOther(_viewModel.ImageBack)
                };

                imageBackBack.sprite = imageBack.sprite;

                imageCard.sprite = _viewModel.RewardType switch
                {
                    RewardType.Road => _imageManager.GetRoad(_viewModel.ImageCard),
                    RewardType.TowerLevelUp => _imageManager.GetTowerCard(_viewModel.ImageCard, _viewModel.Level + 1),
                    RewardType.Tower => _imageManager.GetTowerCard(_viewModel.ImageCard, _viewModel.Level),
                    _ => _imageManager.GetOther(_viewModel.ImageCard),
                };

                imageCardTowerBack.sprite = imageCard.sprite;
            });

            HideCard();
            _disposable = d.Build();
        }


        private void OnEnable()
        {
            buttonBuild.onClick.AddListener(OnRequestBuild);
            buttonShowInfo.onClick.AddListener(OnRequestShowInfo);
            buttonHideInfo.onClick.AddListener(OnRequestHideInfo);
        }

        private void OnRequestShowInfo()
        {
            _animator.Play("show_info_card");
        }

        private void OnRequestHideInfo()
        {
            _animator.Play("hide_info_card");
        }

        private void OnRequestBuild()
        {
            _viewModel.RequestBuild();
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }

        public void ShowCard()
        {
            gameObject.SetActive(true);
            _animator.gameObject.SetActive(true);
            _animator.Play("start_card");
        }

        public void HideCard()
        {
            gameObject.SetActive(false);
        }
    }
}