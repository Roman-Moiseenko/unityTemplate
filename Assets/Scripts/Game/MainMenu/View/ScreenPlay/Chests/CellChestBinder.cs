using System;
using Game.Common;
using Game.GameRoot.ImageManager;
using Game.State.Inventory.Chests;
using Newtonsoft.Json;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenPlay.Chests
{
    
    public class CellChestBinder : MonoBehaviour
    {
        [SerializeField] private Button buttonCell;
        [SerializeField] private Transform showContainer;
        [SerializeField] private Image imageBack;
        [SerializeField] private Image imageChest;
        
        [SerializeField] private Transform statusClose;
        [SerializeField] private Transform toOpening;
        [SerializeField] private Transform isClosed;
        [SerializeField] private TMP_Text timeChest;
        [SerializeField] private TMP_Text levelChest;
        
        [SerializeField] private Transform statusOpening;
        [SerializeField] private TMP_Text timeLeft;
        [SerializeField] private TMP_Text costOpen;
        
        [SerializeField] private Transform statusOpened;
 
        private CellChestViewModel _viewModel;

        private static readonly Color Closed = new Color(0.03f, 0.82f, 1f);
        private static readonly Color Opening = new Color(0.21f, 0.99f, 0.27f);
        private static readonly Color Opened = new Color(1f, 0.92f, 0f);


        private DisposableBag _disposables;
        private DisposableBag _bagOne = new();
        private DisposableBag _bagTwo = new();
        
        public void Bind(CellChestViewModel viewModel)
        {
            //var d = Disposable.CreateBuilder();
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            _viewModel = viewModel;
            viewModel.ChestEntity.Subscribe(chest =>
            {
                
                if (chest == null)
                {
                    showContainer.gameObject.SetActive(false);
                    _bagOne.Dispose();
                    _bagTwo.Dispose();
                }
                else
                {
                    _bagOne.Dispose();
                    _bagTwo.Dispose();
                    _bagOne = new DisposableBag();
                    chest.Status.Subscribe(status =>
                    {
                        if (!this || !gameObject) return;
                        imageBack.color = status switch
                        {
                            StatusChest.Close => Closed,
                            StatusChest.Opening => Opening,
                            StatusChest.Opened => Opened,
                            _ => Closed
                        };
                        
                        statusClose.gameObject.SetActive(false);
                        statusOpening.gameObject.SetActive(false);
                        statusOpened.gameObject.SetActive(false);
                        
                      //  Debug.Log("Сундук - " + chest.Cell + " " + status);
                        //От статуса заполняем и показываем разные блоки
                        
                        switch (status)
                        {
                            case StatusChest.Close:
                                _bagTwo.Dispose();
                                _bagTwo = new DisposableBag();
                                statusClose.gameObject.SetActive(true);
                                viewModel.IsOpening.Subscribe(v =>
                                {
                                    statusClose.gameObject.SetActive(true);
                                    toOpening.gameObject.SetActive(v);
                                    isClosed.gameObject.SetActive(!v);
                                }).AddTo(ref _bagTwo);
                                
                                timeChest.text = $"{chest.TypeChest.FullHourOpening()}ч";
                                levelChest.text = $"Глава {chest.MapId}";
                                
                                break;
                            case StatusChest.Opening:
                                _bagTwo.Dispose();
                                
                                
                                statusOpening.gameObject.SetActive(true);
                                break;
                            case StatusChest.Opened:
                                _bagTwo.Dispose();
                                statusOpened.gameObject.SetActive(true);
                                break;
                            //default: throw new ArgumentOutOfRangeException(nameof(status), status, null);
                        }
                    }).AddTo(ref _bagOne);

                    
                    imageChest.sprite = imageManager.GetChest(viewModel.ChestEntity.Value.TypeChest);
                    //TODO Тексты и Показываем картинки и др.
                    showContainer.gameObject.SetActive(true);
                }
                
            }).AddTo(ref _disposables);
            viewModel.TimeLeft.Where(x => x != 0).Subscribe(t =>
            {
                timeLeft.text = t / 60 > 0 ? $"{t / 60}ч {t % 60}мин" : $"{t % 60}мин";
            }).AddTo(ref _disposables);
            viewModel.CostLeft.Where(x => x!= 0).Subscribe(c =>
                costOpen.text = $"{c}"
                ).AddTo(ref _disposables);
           // _disposable = d.Build();
        }

        private void Update()
        {
            //TODO Если есть StatusChest.Opening сундуки, обновляем таймер
        }

        private void OnEnable()
        {
            buttonCell.onClick.AddListener(OnClickCellChest);
        }

        private void OnDisable()
        {
            buttonCell.onClick.RemoveListener(OnClickCellChest);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
            _bagOne.Dispose();
            _bagTwo.Dispose();
        }

        private void OnClickCellChest()
        {
            _viewModel.RequestClickCellChest();
        }
    }
}