using System;
using Game.Common;
using Game.GameRoot.ImageManager;
using Game.GameRoot.View.Defence;
using Game.State.Gameplay;
using Game.State.Gameplay.Rewards;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelBuild.CardFrontend
{
    /**
     * Передняя сторона карточки. Состоит из 3 блоков:
     * Фон карточки, Изображение сущности, Название Сущности - постоянный блок 
     * Блок Upgrade - для Tower/Scill/Hero Upgrade (хар-ки, звездочки, инфо, анимация)
     * Блок Tower - размещение башни (Defence и Levels)
     */
    public class CardFrontendBinder : MonoBehaviour
    {
        //Общие данные
        [SerializeField] private TMP_Text textDescription;
        [SerializeField] private Image imageCard;
        [SerializeField] private Image imageBack;
        [SerializeField] public Button infoButton;
        
        //По типам карточки/награды
        [SerializeField] private FrontendUpgradeBinder upgradeBinder;
        [SerializeField] private FrontendTowerBinder towerBinder;
        
        private IDisposable _disposable;
        private ImageManagerBinder _imageManager;
        
        private void Awake()
        {
            _imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
        }
        
        public void Bind(CardViewModel viewModel)
        {
            
            upgradeBinder.Bind(viewModel);
            towerBinder.Bind(viewModel);
            
            var d = Disposable.CreateBuilder();
            
            imageCard.color = Color.white;
            viewModel.Updated.Subscribe(_ =>
            {

                //Меняем фон кнопки Инфо карточки
                if (viewModel.RewardType.IsUpgrade())
                {
                    infoButton.GetComponent<Image>().sprite = _imageManager.GetOther("UpgradeBtnCard");
                }
                else
                {
                    infoButton.GetComponent<Image>().sprite = _imageManager.GetOther("OtherBtnCard");
                }
                
                textDescription.text = viewModel.Description;
                //Фон карточки
                imageBack.sprite = viewModel.RewardType switch
                {
                    RewardType.Tower => _imageManager.GetEpicLevel(viewModel.ImageBack),
                    _ => _imageManager.GetOther(viewModel.ImageBack)
                };
                textDescription.gameObject.SetActive(!viewModel.RewardType.IsUpgrade());

                imageCard.sprite = viewModel.RewardType switch
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