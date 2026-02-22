using System;
using System.Collections.Generic;
using Game.Common;
using Game.GameRoot.ImageManager;
using Game.State.Gameplay;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelBuild.CardBackend
{
    public class CardBackendBinder : MonoBehaviour
    {
        [SerializeField] private TMP_Text textDescriptionBack;
        [SerializeField] private Image imageBackBack;
        [SerializeField] private Image imageCardBack;
        [SerializeField] private Transform blockParameters;
        [SerializeField] private List<CardParameterBinder> parameterBinders; 
        [SerializeField] private Transform upgradeInfo;
        [SerializeField] public Image imageReturn;
        
        private IDisposable _disposable;
        private ImageManagerBinder _imageManager;

        
        private RectTransform _imageCardBackTransform;
        private RectTransform _textDescriptionTransform;
        private readonly Vector3 _positionCardTower = new(0, 320, 0);
        private readonly Vector2 _sizeCardTower = new(200, 200);
        private readonly Vector3 _positionTextDescriptionTower = new(0, 50, 0);
        
        private readonly Vector3 _positionCardOther = new(0, 150, 0);
        private readonly Vector2 _sizeCardOther = new(200, 200);
        private readonly Vector3 _positionTextDescriptionOther = new(0, -120, 0);
        private void Awake()
        {
            _imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
        }

        public void Bind(CardViewModel viewModel)
        {
            imageCardBack.color = Color.white;
            _imageCardBackTransform = imageCardBack.GetComponent<RectTransform>();
            _textDescriptionTransform = textDescriptionBack.GetComponent<RectTransform>();
            
            foreach (var parameterBinder in parameterBinders)
            {
                parameterBinder.gameObject.SetActive(false);
            }
            
            var d = Disposable.CreateBuilder();
            viewModel.Updated.Subscribe(_ =>
            {
                //Фон карточки
                imageBackBack.sprite = viewModel.RewardType switch
                {
                    RewardType.Tower => _imageManager.GetEpicLevel(viewModel.ImageBack),
                    _ => _imageManager.GetOther(viewModel.ImageBack)
                };
                
                //Меняем фон кнопки Инфо карточки
                if (viewModel.RewardType.IsUpgrade())
                {
                    imageReturn.sprite = _imageManager.GetOther("UpgradeBtnCard");
                }
                else
                {
                    imageReturn.sprite = _imageManager.GetOther("OtherBtnCard");
                }
                
                //Показываем Инво блок, что это Улучшение 
                if (viewModel.RewardType is RewardType.TowerLevelUp or RewardType.HeroLevelUp or RewardType.SkillLevelUp)
                {
                    upgradeInfo.gameObject.SetActive(true);
                }
                else
                {
                    upgradeInfo.gameObject.SetActive(false);
                }
                
                
                if (viewModel.RewardType is RewardType.Tower or RewardType.TowerLevelUp)
                {
                    _imageCardBackTransform.localPosition = _positionCardTower;
                    _imageCardBackTransform.sizeDelta = _sizeCardTower;
                    _textDescriptionTransform.localPosition = _positionTextDescriptionTower;
                    
                    blockParameters.gameObject.SetActive(true);
                    //Заполняем блок параметров
                    var index = 0;
                    foreach (var paramData in viewModel.Parameters)
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


                textDescriptionBack.text = viewModel.DescriptionBack;
                imageCardBack.sprite = viewModel.RewardType switch
                {
                    RewardType.Road => _imageManager.GetRoad(viewModel.ImageCard),
                    RewardType.TowerLevelUp => _imageManager.GetTowerCard(viewModel.ImageCard, viewModel.NumberModel),
                    RewardType.Tower => _imageManager.GetTowerCard(viewModel.ImageCard, viewModel.NumberModel),
                    _ => _imageManager.GetOther(viewModel.ImageCard),
                };
                
                
            }).AddTo(ref d);
            _disposable = d.Build();
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}