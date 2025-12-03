using System;
using System.Collections.Generic;
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
        [SerializeField] private Image imageCardBack;
        [SerializeField] private Transform blockParameters;
        [SerializeField] private List<CardParameterBinder> parameterBinders; 

        [SerializeField] private Transform blockLevel;
        [SerializeField] private Transform blockLevelUp;
        
        
        private RectTransform _imageCardBackTransform;
        private RectTransform _textDescriptionTransform;
        private readonly Vector3 _positionCardTower = new(0, 240, 0);
        private readonly Vector2 _sizeCardTower = new(150, 150);
        private readonly Vector3 _positionTextDescriptionTower = new(0, 50, 0);
        
        private readonly Vector3 _positionCardOther = new(0, 150, 0);
        private readonly Vector2 _sizeCardOther = new(200, 200);
        private readonly Vector3 _positionTextDescriptionOther = new(0, -120, 0);


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
            imageCard.color = Color.white;
            imageCardBack.color = Color.white;
            _imageCardBackTransform = imageCardBack.GetComponent<RectTransform>();
            _textDescriptionTransform = textDescriptionBack.GetComponent<RectTransform>();
            buttonHideInfo.GetComponent<RectTransform>().gameObject.SetActive(false);
            foreach (var parameterBinder in parameterBinders)
            {
                parameterBinder.gameObject.SetActive(false);
            }
            
            _animator = gameObject.GetComponent<Animator>();
            _viewModel = viewModel;
            _viewModel.Updated.Subscribe(_ =>
            {
                textCaption.text = _viewModel.Caption;
                textDescription.text = _viewModel.Description;
                if (_viewModel.Defence != null)
                {
                    imageDefence.sprite = _imageManager.GetDefence(_viewModel.Defence);
                    imageDefence?.gameObject.SetActive(true);
                }
                else
                {
                    imageDefence?.gameObject.SetActive(false);
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
                if (_viewModel.RewardType is RewardType.Tower or RewardType.TowerLevelUp)
                {
                    _imageCardBackTransform.localPosition = _positionCardTower;
                    _imageCardBackTransform.sizeDelta = _sizeCardTower;
                    _textDescriptionTransform.localPosition = _positionTextDescriptionTower;
                    
                    blockParameters.gameObject.SetActive(true);
                    //Заполняем блок параметров
                    var index = 0;
                    foreach (var paramData in _viewModel.Parameters)
                    {
                        parameterBinders[index].Bind(paramData.Key, paramData.Value);
                        index++;
                        if (index >= 4) break;
                    }
                }
                else
                {
                    _imageCardBackTransform.localPosition = _positionCardOther;
                    _imageCardBackTransform.sizeDelta = _sizeCardOther;
                    _textDescriptionTransform.localPosition = _positionTextDescriptionOther;
                    
                    blockParameters.gameObject.SetActive(false);
                }

                if (_viewModel.RewardType == RewardType.Tower)
                {
                    blockLevel.GetComponent<LevelBinder>().Bind(_viewModel.Level);
                    blockLevel.gameObject.SetActive(true);
                } else
                {
                    blockLevel.gameObject.SetActive(false);
                }
                
                if (_viewModel.RewardType == RewardType.TowerLevelUp)
                {
                    blockLevelUp.gameObject.SetActive(true);
                    blockLevelUp.GetComponent<LevelUpBinder>().Show(_viewModel.Level);
                } else
                {
                    blockLevelUp.GetComponent<LevelUpBinder>().Hide();
                }
                textDescriptionBack.text = _viewModel.DescriptionBack;
                imageCardBack.sprite = imageCard.sprite;
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

        private void OnDisable()
        {
            buttonBuild.onClick.RemoveListener(OnRequestBuild);
            buttonShowInfo.onClick.RemoveListener(OnRequestShowInfo);
            buttonHideInfo.onClick.RemoveListener(OnRequestHideInfo);
        }

        private void OnRequestShowInfo()
        {
            _animator.Play("show_info_card");
//            var t = _textDescriptionTransform.GetComponent<RectTransform>();
          //  Debug.Log(t.sizeDelta + " " + t.localPosition + " "  + t.name);
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