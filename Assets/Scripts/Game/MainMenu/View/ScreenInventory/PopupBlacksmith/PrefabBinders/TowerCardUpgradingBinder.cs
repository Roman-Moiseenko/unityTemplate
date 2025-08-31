using System;
using Game.Common;
using Game.GameRoot.ImageManager;
using Game.MainMenu.View.ScreenInventory.TowerCards;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace Game.MainMenu.View.ScreenInventory.PopupBlacksmith.PrefabBinders
{
    public class TowerCardUpgradingBinder : MonoBehaviour
    {
        private TowerCardUpgradingViewModel _viewModel;
        private IDisposable _disposable;

        [SerializeField] private Image epicImage;
        [SerializeField] private Image towerImage;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private Button buttonReset;
        private Vector3 basePosition;
        [SerializeField] private Image setImage;

        [SerializeField] private Transform baseParent;
        [SerializeField] private Transform movingParent;

        //TODO Отслеживать и перемещать модель в Binder 
        public ReactiveProperty<bool> IsInDeck = new(false);
        private Vector3 _target;
        private bool _IsMoving = false;

        public void Bind(TowerCardUpgradingViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            basePosition = transform.position;

            _viewModel = viewModel;
            //   _viewModel.Position = Position;

            viewModel.IsNecessary.Subscribe(v =>
            {
                if (v)
                {
                    epicImage.sprite = imageManager.GetEpicLevel(viewModel.EpicLevel);
                    levelText.text = $"Ур. {viewModel.Level}";
                    towerImage.sprite = imageManager.GetTowerCard(viewModel.ConfigId, 1);
                    gameObject.SetActive(true);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            });

            viewModel.IsSetCard.Subscribe(v =>
            {
                if (v)
                {
                    levelText.text = $"Ур. {viewModel.Level}";
                    //TODO П
                    transform.position = viewModel.PositionResource;
                    transform.SetParent(movingParent);
                    _target = basePosition;
                    _IsMoving = true;

                  //  Debug.Log("basePosition = " + basePosition);
                   // Debug.Log("viewModel.PositionResource = " + viewModel.PositionResource);
                }


                setImage.gameObject.SetActive(!v);
            });


            //     transform.GetComponent<RectTransform>().anchoredPosition = viewModel.Position;
            _disposable = d.Build();
        }

        private void Update()
        {
            if (!_IsMoving) return;
            
            transform.position = Vector3.Lerp(transform.position, _target, Time.deltaTime * 10);
            if (transform.position == _target)
            {
                _IsMoving = false;
                transform.SetParent(baseParent);
            }
        }

        private void OnEnable()
        {
            buttonReset.onClick.AddListener(OnResetTowerCard);
        }

        private void OnDisable()
        {
            buttonReset.onClick.RemoveListener(OnResetTowerCard);
        }

        public void OnResetTowerCard()
        {
            _viewModel.ResetTowerCard();
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}