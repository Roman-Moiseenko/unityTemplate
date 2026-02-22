using System;
using DG.Tweening;
using Game.Common;
using Game.GamePlay.View.UI.PanelBuild.CardBackend;
using Game.GamePlay.View.UI.PanelBuild.CardFrontend;
using Game.GameRoot.ImageManager;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelBuild
{
    /**
     * Главный Binder Карточки. Отвечает за запуск передней и задней стороны, за смену сторон
     */
    public class CardBinder : MonoBehaviour
    {
        [SerializeField] private CardFrontendBinder frontendBinder;
        [SerializeField] private CardBackendBinder backendBinder;
        
        private CardViewModel _viewModel;
        Sequence Sequence;

        private void Awake()
        {
           // _imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
        }

        public void Bind(CardViewModel viewModel)
        {
            _viewModel = viewModel;
            frontendBinder.Bind(_viewModel);
            backendBinder.Bind(_viewModel);
            backendBinder.gameObject.SetActive(false);
            frontendBinder.gameObject.SetActive(true);
            HideCard();
        }


        private void OnEnable()
        {
            frontendBinder.GetComponent<Button>().onClick.AddListener(OnRequestBuild);
            frontendBinder.infoButton.GetComponent<Button>().onClick.AddListener(OnRequestShowInfo);
            backendBinder.GetComponent<Button>().onClick.AddListener(OnRequestHideInfo);
        }

        private void OnDisable()
        {
            frontendBinder.GetComponent<Button>().onClick.RemoveListener(OnRequestBuild);
            frontendBinder.infoButton.GetComponent<Button>().onClick.RemoveListener(OnRequestShowInfo);
            backendBinder.GetComponent<Button>().onClick.RemoveListener(OnRequestHideInfo);
        }

        private void OnRequestShowInfo()
        {
            Sequence = DOTween.Sequence();
            Sequence
                .Append(frontendBinder.transform
                    .DOLocalRotate(new Vector3(0, 90, 0), 0.15f)
                    .From(Vector3.zero).SetUpdate(true))
                .AppendCallback(() =>
                {
                    backendBinder.gameObject.SetActive(true);
                    frontendBinder.gameObject.SetActive(false);
                })
                .Append(backendBinder.transform
                    .DOLocalRotate(new Vector3(0, 0, 0), 0.15f)
                    .From(new Vector3(0, 90, 0)).SetUpdate(true))
                .OnComplete(() =>
                {
                    Sequence.Kill();
                })  
                .SetUpdate(true);
        }

        private void OnRequestHideInfo()
        {
            Sequence = DOTween.Sequence();
            Sequence
                .Append(backendBinder.transform
                    .DORotate(new Vector3(0, 90, 0), 0.15f)
                    .From(Vector3.zero).SetUpdate(true))
                .AppendCallback(() =>
                {
                    backendBinder.gameObject.SetActive(false);
                    frontendBinder.gameObject.SetActive(true);
                })
                .Append(frontendBinder.transform
                    .DORotate(new Vector3(0, 0, 0), 0.15f)
                    .From(new Vector3(0, 90, 0)).SetUpdate(true))
                .OnComplete(() => { Sequence.Kill(); })  
                .SetUpdate(true);
        }

        private void OnRequestBuild()
        {
            _viewModel.RequestBuild();
        }

        private void OnDestroy()
        {
            if (Sequence.IsActive())
            {
                Sequence.Kill();
                Sequence = null;
            }
            
        }

        public void ShowCard()
        {
            gameObject.SetActive(true);
            transform.DOScale(1, 0.1f).From(0.8f).SetEase(Ease.OutSine).SetUpdate(true);
        }

        public void HideCard()
        {
            gameObject.SetActive(false);
        }
    }
}