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
        
        private IDisposable _disposable;
        private CellChestViewModel _viewModel;

        private static readonly Color closed = new Color(0.03f, 0.82f, 1f);
        private static readonly Color opening = new Color(0.21f, 0.99f, 0.27f);
        private static readonly Color opened = new Color(1f, 0.92f, 0f);
        

        public void Bind(CellChestViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            _viewModel = viewModel;
            viewModel.ChestEntity.Subscribe(chest =>
            {
                IDisposable dd = null;
                if (chest == null)
                {
                    showContainer.gameObject.SetActive(false);
                    if (dd != null) dd.Dispose();
                }
                else
                {
                    dd = chest.Status.Subscribe(status =>
                    {

                        imageBack.color = status switch
                        {
                            StatusChest.Close => closed,
                            StatusChest.Opening => opening,
                            StatusChest.Opened => opened,
                            _ => closed
                        };
                        
                        statusClose.gameObject.SetActive(false);
                        statusOpening.gameObject.SetActive(false);
                        statusOpened.gameObject.SetActive(false);
                        
                      //  Debug.Log("Сундук - " + chest.Cell + " " + status);
                        //От статуса заполняем и показываем разные блоки
                        IDisposable d = null;
                        switch (status)
                        {
                            case StatusChest.Close:
                                d = viewModel.IsOpening.Subscribe(v =>
                                {
                                    toOpening.gameObject.SetActive(v);
                                    isClosed.gameObject.SetActive(!v);
                                });
                                
                                timeChest.text = $"{chest.TypeChest.FullHourOpening()}ч";
                                levelChest.text = $"Уровень {chest.Level}";
                                statusClose.gameObject.SetActive(true);
                                break;
                            case StatusChest.Opening:
                                if (d != null) d.Dispose();
                                
                                statusOpening.gameObject.SetActive(true);
                                break;
                            case StatusChest.Opened:
                                if (d != null) d.Dispose();
                                statusOpened.gameObject.SetActive(true);
                                break;
                            //default: throw new ArgumentOutOfRangeException(nameof(status), status, null);
                        }
                    });

                    
                    imageChest.sprite = imageManager.GetChest(viewModel.ChestEntity.Value.TypeChest);
                    //TODO Тексты и Показываем картинки и др.
                    showContainer.gameObject.SetActive(true);
                }
                
            }).AddTo(ref d);
            viewModel.TimeLeft.Where(x => x != 0).Subscribe(t =>
            {
                timeLeft.text = t / 60 > 0 ? $"{t / 60}ч {t % 60}мин" : $"{t % 60}мин";
            }).AddTo(ref d);
            viewModel.CostLeft.Where(x => x!= 0).Subscribe(c =>
                costOpen.text = $"{c}"
                ).AddTo(ref d);
            _disposable = d.Build();
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
            _disposable.Dispose();
        }

        private void OnClickCellChest()
        {
            _viewModel.RequestClickCellChest();
        }
    }
}