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
                if (chest == null)
                {
                    showContainer.gameObject.SetActive(false);
                }
                else
                {
                    imageBack.color = chest.Status.Value switch
                    {
                        StatusChest.Close => closed,
                        StatusChest.Opening => opening,
                        StatusChest.Opened => opened,
                        _ => imageBack.color
                    };

                    chest.Status.Subscribe(status =>
                    {

                        statusClose.gameObject.SetActive(false);
                        statusOpening.gameObject.SetActive(false);
                        statusOpened.gameObject.SetActive(false);
                        //От статуса заполняем и показываем разные блоки
                        switch (status)
                        {
                            case StatusChest.Close:
                                viewModel.IsOpening.Subscribe(v =>
                                {
                                    toOpening.gameObject.SetActive(v);
                                    isClosed.gameObject.SetActive(!v);
                                });
                                
                                timeChest.text = $"{chest.TypeChest.TimeOut()}ч";
                                levelChest.text = $"Уровень {chest.Level}";
                                statusClose.gameObject.SetActive(true);
                                break;
                            case StatusChest.Opening:
                                statusOpening.gameObject.SetActive(true);
                                break;
                            case StatusChest.Opened:
                                statusOpened.gameObject.SetActive(true);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(status), status, null);
                        }
                    });

                    
                    imageChest.sprite = imageManager.GetChest(viewModel.ChestEntity.Value.TypeChest);
                    //TODO Тексты и Показываем картинки и др.
                    showContainer.gameObject.SetActive(true);
                }
                
            }).AddTo(ref d);
            
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