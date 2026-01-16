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
        [SerializeField] private List<Transform> cards;

        private Transform _freeCaption;
        private Transform _paidCaption;
        private TMP_Text _paidText;

        private IDisposable _disposable;
        private ImageManagerBinder _imageManager;
        Sequence Sequence;

        private void Awake()
        {
            _freeCaption = _btnUpdate.transform.Find("Free");
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
            {
                var binder = cards[i - 1].GetComponent<CardBinder>();
                binder.Bind(viewModel.CardViewModels[i]);
            }
            viewModel.UpdateCards.Subscribe(value =>
            {
                if (value == 0)
                {
                    _freeCaption.gameObject.SetActive(true);
                    _paidCaption.gameObject.SetActive(false);
                }
                else
                {
                    _freeCaption.gameObject.SetActive(false);
                    _paidCaption.gameObject.SetActive(true);
                    _paidText.text = (value * AppConstants.COST_UPDATE_BUILD).ToString();
                }
            }).AddTo(ref d);
            
            _disposable = d.Build();
        }

        private IEnumerator pause()
        {
            yield return new WaitForSeconds(0.1f);
            Time.timeScale = 0;
        }

        public override void Show()
        {
            if (isShow) return;
            //Получаем у ViewModel данные для отображения на карточках, грузим картинки
            base.Show();
            panel.pivot = new Vector2(0.5f, 0);
            StartAnimationCards();
            //StartCoroutine(ShowCards());
        }

        private void StartAnimationCards()
        {
            //Debug.Log("StartAnimationCards");
            Sequence = DOTween.Sequence();
            Sequence
                .AppendCallback(() => cards[0].GetComponent<CardBinder>().ShowCard())
                .AppendInterval(0.1f)
                //.Append(DOTween.Sequence().SetDelay(0.1f).SetUpdate(true))
                .AppendCallback(() => cards[1].GetComponent<CardBinder>().ShowCard())
                .AppendInterval(0.1f)
                //.Append(DOTween.Sequence().SetDelay(0.1f).SetUpdate(true))
                .AppendCallback(() => cards[2].GetComponent<CardBinder>().ShowCard())
                .OnComplete(() =>
                {
                    Sequence.Kill();
                }).SetUpdate(true);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator ShowCards()
        {
            foreach (var card in cards)
            {
                card.GetComponent<CardBinder>().ShowCard();
                yield return new WaitForSeconds(0.1f);
            }
        }

        public override void Hide()
        {
            if (!isShow) return;
            base.Hide();
            foreach (var card in cards)
            {
                card.GetComponent<CardBinder>().HideCard();
            }

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
            ViewModel.OnUpdateCard();
        }

        public void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}