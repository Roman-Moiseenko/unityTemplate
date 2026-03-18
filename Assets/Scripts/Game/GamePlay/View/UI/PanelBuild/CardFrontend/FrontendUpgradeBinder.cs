using System;
using System.Collections.Generic;
using Game.Common;
using Game.GameRoot.ImageManager;
using Game.State.Gameplay;
using Game.State.Gameplay.Rewards;
using R3;
using TMPro;
using UnityEngine;

namespace Game.GamePlay.View.UI.PanelBuild.CardFrontend
{
    public class FrontendUpgradeBinder : MonoBehaviour
    {
                
        
        [SerializeField] private Transform upgradeLevel;
      //  [SerializeField] private Transform upgradeInfo;
        [SerializeField] private Transform upgradeDescription;
        [SerializeField] private List<UpgradeArrowBinder> arrowBinders;
        [SerializeField] private List<UpgradeParameterBinder> parameterBinders;
        private IDisposable _disposable;
      //  private ImageManagerBinder _imageManager;
        
        private void Awake()
        {
           // _imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
        }
        
        public void Bind(CardViewModel viewModel)
        {
            foreach (var arrowBinder in arrowBinders)
            {
                arrowBinder.Bind();
            }
            
            var d = Disposable.CreateBuilder();
            viewModel.Updated.Subscribe(_ =>
            {
                if (!viewModel.RewardType.IsUpgrade())
                {
                    gameObject.SetActive(false);
                    return;
                }

                foreach (var parameterBinder in parameterBinders)
                {
                    parameterBinder.gameObject.SetActive(false);
                }

                var index = 0;
                foreach (var (parameter, value) in viewModel.UpgradeParameters)
                {
                    parameterBinders[index].Bind(value, parameter);
                    index++;
                }
                

                upgradeLevel.gameObject.SetActive(true);
                //TODO в зависимоти от типа показать кол-во зведочек 6(Tower) или 3(Skill/Hero)
                
                upgradeLevel.GetComponent<LevelUpBinder>().Show(viewModel.Level);
                //upgradeDescription вставить описание списком и цветом 
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