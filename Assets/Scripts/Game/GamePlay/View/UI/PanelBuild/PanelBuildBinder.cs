using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Common;
using Game.GameRoot.ImageManager;
using MVVM.UI;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelBuild
{
    public class PanelBuildBinder : PanelBinder<PanelBuildViewModel>
    {
        [SerializeField] private Button _btnUpdate;
        //[SerializeField] private List<Transform> cards;
        [SerializeField] private List<CardBinder> cardBinders;

        private Transform _freeCaption;
        private Transform _freeAdCaption;
        private Transform _paidCaption;
        private TMP_Text _paidText;

        private IDisposable _disposable;
        private ImageManagerBinder _imageManager;

        private void Awake()
        {
            _freeCaption = _btnUpdate.transform.Find("Free");
            _freeAdCaption = _btnUpdate.transform.Find("FreeAd");
            _paidCaption = _btnUpdate.transform.Find("Paid");
            _paidText = _paidCaption
                .transform.Find("ImageBlock")
                .transform.Find("costText")
                .gameObject.GetComponent<TMP_Text>();
            _imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
        }

        
        protected override void OnBind(PanelBuildViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _freeCaption.gameObject.SetActive(true);
            _paidCaption.gameObject.SetActive(false);

            for (var i = 1; i <= 3; i++)
                cardBinders[i - 1].Bind(viewModel.CardViewModels[i]);
            _freeCaption.gameObject.SetActive(false);
            _freeAdCaption.gameObject.SetActive(false);
            _paidCaption.gameObject.SetActive(false);
            
            viewModel.UpdateCards.Subscribe(value =>
            {
                if (value == 0)
                {
                    _freeCaption.gameObject.SetActive(true);
                }

                if (value == 1)
                {
                    _freeCaption.gameObject.SetActive(false);
                    _freeAdCaption.gameObject.SetActive(true);
                }
                if (value > 1)
                {
                    _freeAdCaption.gameObject.SetActive(false);
                    _paidCaption.gameObject.SetActive(true);
                    _paidText.text = ((value - 1) * AppConstants.COST_UPDATE_BUILD).ToString();
                }
                //TODO Развернуть карты
            }).AddTo(ref d);
            
            _disposable = d.Build();
        }

        public override void Show()
        {
            //Получаем у ViewModel данные для отображения на карточках, грузим картинки
            panel.pivot = new Vector2(0.5f, 0);
            StartCoroutine(ShowCards());
        }
        
        private IEnumerator ShowCards()
        {
            foreach (var card in cardBinders)
            {
                card.ShowCard();
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }

        public override void Hide()
        {
            foreach (var card in cardBinders)
                card.HideCard();

            panel.pivot = new Vector2(0.5f, 1);
        }

        private void OnEnable()
        {
            _btnUpdate.onClick.AddListener(OnClickUpdate);
        }

        private void OnDisable()
        {
            _btnUpdate.onClick.RemoveListener(OnClickUpdate);
        }

        private void OnClickUpdate()
        {
            foreach (var cardBinder in cardBinders)
                cardBinder.ReturnFrontendShow();
            
            ViewModel.OnUpdateCard();
        }

        public void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}