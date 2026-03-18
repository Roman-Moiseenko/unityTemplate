using System;
using System.Collections.Generic;
using Game.Common;
using Game.GameRoot.ImageManager;
using Game.State.Gameplay;
using Game.State.Gameplay.Rewards;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.UI.PanelBuild.CardBackend
{
    public class BackendParametersBinder : MonoBehaviour
    {
        [SerializeField] private List<BackendParameterBinder> parameterBinders;
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
                if (!viewModel.RewardType.IsTower() && !viewModel.RewardType.IsUpgrade())
                {
                    transform.gameObject.SetActive(false);
                    return;
                }
                foreach (var parameterBinder in parameterBinders)
                    parameterBinder.gameObject.SetActive(false);
                
                //Заполняем блок параметров
                var index = 0;
                foreach (var paramData in viewModel.InfoCardParameters)
                {
                    parameterBinders[index].Bind(paramData.Key, paramData.Value);
                    index++;
                    if (index >= 4) break;
                }
                
                transform.gameObject.SetActive(true);
            }).AddTo(ref d);
            _disposable = d.Build();
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}