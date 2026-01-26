using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Common;
using Game.GamePlay.Queries.Classes;
using Game.GameRoot.ImageManager;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelGateWave.InfoWave
{
    public class InfoWaveBinder : MonoBehaviour
    {
        [SerializeField] private Button btnInfoWave;
        [SerializeField] private Image iconBoss;
        [SerializeField] private Image iconEnemy;
        [SerializeField] private Image selected;
        [SerializeField] private Image timer;
        [SerializeField] private Transform infoPanel;
        [SerializeField] private Transform containerEnemies;
        
        
        private InfoWaveViewModel _viewModel;
        private ImageManagerBinder _imageManager;
        private IDisposable _disposable;
        
        private List<EnemyInfoBinder> _enemyInfoBinders = new();
        private ReactiveProperty<bool> _isBoss = new(false);
        public void Bind(InfoWaveViewModel viewModel)
        {
            _viewModel = viewModel;
            _imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            var d = Disposable.CreateBuilder();
            
            selected.gameObject.SetActive(false);
            btnInfoWave.gameObject.SetActive(false);
            infoPanel.gameObject.SetActive(false);
            _isBoss.Subscribe(v =>
            {
                iconEnemy.gameObject.SetActive(!v);
                iconBoss.gameObject.SetActive(v);
            }).AddTo(ref d);

  
            //Кнопка Показать Инфо о волне
            viewModel.ShowButtonWave.Subscribe(showButton =>
            {
                if (showButton)
                {
                    //Меняем круглешок на кнопке Волны (враги или босс)
                    
                    btnInfoWave.gameObject.SetActive(true);
                    btnInfoWave.transform.DOScale(1, 0.3f)
                        .From(0.7f)
                        .SetEase(Ease.OutBack)
                        .SetUpdate(true);
                }
                else
                {
                    btnInfoWave.transform.DOScale(0f, 0.3f)
                        .From(1)
                        .SetEase(Ease.InBack)
                        .SetUpdate(true)
                        .OnComplete(() =>
                        {
                            btnInfoWave.gameObject.SetActive(false);
                            timer.fillAmount = 1;
                        });
                }

                viewModel.ShowInfoWave.OnNext(false);
            }).AddTo(ref d);
            viewModel.PositionInfoBtn.Subscribe(p => transform.position = p).AddTo(ref d);
            viewModel.FillAmountBtn.Subscribe(n => timer.fillAmount = n).AddTo(ref d);
           // _infoPanel.gameObject.SetActive(false);
           //подписка при Показать/скрыть Инфо о волне 
           viewModel.ShowInfoWave.Skip(1).Subscribe(showInfo =>
            {
                if (showInfo)
                {
                    infoPanel.gameObject.SetActive(true);
                    infoPanel.DOScale(1, 0.3f)
                        .From(0.3f)
                        .SetEase(Ease.OutBack)
                        .SetUpdate(true);
                }
                else
                {
                    infoPanel.DOScale(0f, 0.3f)
                        .From(1)
                        .SetEase(Ease.InBack)
                        .SetUpdate(true)
                        .OnComplete(() => infoPanel.gameObject.SetActive(false));
                    
                }
                selected.gameObject.SetActive(showInfo);
            }).AddTo(ref d);
            InfoWavePanelClear();
            //Заполняем для первой волны
            foreach (var defenceCountMob in viewModel.AllEnemyDataInfo)
            {
                InfoWavePanelAddEntity(defenceCountMob);
            }

            //При добавлении в последующих волн.
            viewModel.AllEnemyDataInfo.ObserveAdd()
                .Subscribe(e => InfoWavePanelAddEntity(e.Value)).
                AddTo(ref d);
            viewModel.AllEnemyDataInfo.ObserveClear().Subscribe(_ => InfoWavePanelClear()).AddTo(ref d);
            
            _disposable = d.Build();
        }
        
        private void InfoWavePanelAddEntity(EnemyDataInfo enemyDataInfo)
        {
            var prefabPath = $"Prefabs/UI/Gameplay/Panels/GateWaveInfo/EnemyInfo"; //Перенести в настройки уровня
            var enemyPrefab = Resources.Load<EnemyInfoBinder>(prefabPath);
            var createdInfoString = Instantiate(enemyPrefab, containerEnemies);
            var image = _imageManager.GetDefence(enemyDataInfo.Defence);

            createdInfoString.Bind(
                image,
                enemyDataInfo.TitleLid,
                enemyDataInfo.Quantity,
                _enemyInfoBinders.Count,
                enemyDataInfo.IsBoss
            );
            _enemyInfoBinders.Add(createdInfoString);
            containerEnemies.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 310 + 110 * (_enemyInfoBinders.Count - 1));;
            if (enemyDataInfo.IsBoss) _isBoss.Value = false;
        }
        
        private void InfoWavePanelClear()
        {
            foreach (var infoBinder in _enemyInfoBinders)
            {
                Destroy(infoBinder.gameObject);
            }

            _isBoss.Value = false;
            _enemyInfoBinders.Clear();
            containerEnemies.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 270);
        }
        
        private void OnStartForced()
        {
            if (!_viewModel.ShowInfoWave.CurrentValue)
            {
                _viewModel.ShowInfoWave.OnNext(true);
            }
            else
            {
                _viewModel.StartForcedWave();
            }
        }
        
        private void OnEnable()
        {
            btnInfoWave.onClick.AddListener(OnStartForced); 
        }

        private void OnDisable()
        {
            btnInfoWave.onClick.RemoveListener(OnStartForced);
        }
        
        public void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}