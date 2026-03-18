using System;
using Game.Common;
using Game.GameRoot.ImageManager;
using Game.GameRoot.View.Defence;
using Game.State.Gameplay;
using Game.State.Gameplay.Rewards;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.UI.PanelBuild.CardFrontend
{

    public class FrontendTowerBinder : MonoBehaviour
    {
        [SerializeField] private Transform blockLevel;
        [SerializeField] private DefenceBinder defenceBinder;
        
        private IDisposable _disposable;
          private ImageManagerBinder _imageManager;
        
        private void Awake()
        {
             _imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
        }
        public void Bind(CardViewModel viewModel)
        {
            
            var d = Disposable.CreateBuilder();
            viewModel.Updated.Subscribe(_ =>
            {
                if (viewModel.RewardType != RewardType.Tower)
                {
                    gameObject.SetActive(false);
                    return;
                }
                defenceBinder.Bind(viewModel.Defence);
                blockLevel.GetComponent<LevelBinder>().Bind(viewModel.Level); 
                gameObject.SetActive(true);
                
            }).AddTo(ref d);
            _disposable = d.Build();
        } 
        
        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}