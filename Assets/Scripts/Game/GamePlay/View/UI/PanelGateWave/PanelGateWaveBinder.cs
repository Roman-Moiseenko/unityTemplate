using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Common;
using Game.GameRoot.ImageManager;
using Game.State.Maps.Mobs;
using Game.State.Maps.Towers;
using MVVM.UI;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelGateWave
{
    public class PanelGateWaveBinder : PanelBinder<PanelGateWaveViewModel>
    {
        [SerializeField] private Transform _infoBlock;
        [SerializeField] private Transform _infoTower;
      //  [SerializeField] private List<MobDefenceImage> _mobDefenceImages;
      //  [SerializeField] private List<ParameterTypeImage> _parameterImages;
        
        //Компоненты блока о Волне
        private Button _btnInfo;
        private Transform _infoPanel;
        private Transform _enemies;
        //Компоненты блока о Башне
        private Transform _baseParameters;
        private Transform _upgradeParameters;
        
        private List<EnemyInfoBinder> _enemyInfoBinders = new();
        private List<BaseParameterBinder> _baseParameterBinders = new();
        private List<UpgradeParameterBinder> _upgradeParameterBinders = new();
        
        private Image _btnImage;
        private IDisposable _disposableImplementation;
        private ImageManagerBinder _imageManager;

        private void Awake()
        {
            //Инициализация
            //Аниматор панели
            //Кнопка Волны - показ инфо, форсированный запуск
            _btnInfo = _infoBlock.Find("ButtonBlock/ButtonWave").GetComponent<Button>();
            //Инфо панель о волне
            _infoPanel = _infoBlock.Find("InfoWave").GetComponent<Transform>();
            //Контейнер для списка инфо о мобах
            _enemies = _infoBlock.Find("InfoWave/Enemies").GetComponent<Transform>();
            //Индикатор таймера
            _btnImage = _btnInfo.GetComponent<Image>();
            _btnImage.fillAmount = 1;
            //Контейнер для списка инфо о базовых параметрах
            _baseParameters = _infoTower.Find("BaseParameters").GetComponent<Transform>();
            //Контейнер для списка инфо о апгрейднутых параметрах
            _upgradeParameters = _infoTower.Find("UpgrateParameters").GetComponent<Transform>();
            //Базовое отключение
            _infoTower.gameObject.SetActive(false);
            
            //_mobDefenceImages.Add(MobDefence.Advanced, Resources.Load<Texture2D>("MyTexture"));
        }
        protected override void OnBind(PanelGateWaveViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            //Блок инфо о волне
            ViewModel.ShowInfoWave.OnNext(false);
            ViewModel.ShowButtonWave.Subscribe(showButton =>
            {
                if (showButton)
                {
                    _infoBlock.gameObject.SetActive(true);
                    _infoBlock.DOScale(1, 0.3f)
                        .From(0.7f)
                        .SetEase(Ease.OutBack)
                        .SetUpdate(true);
                }
                else
                {
                    _infoBlock.DOScale(0f, 0.3f)
                        .From(1)
                        .SetEase(Ease.InBack)
                        .SetUpdate(true)
                        .OnComplete(() =>
                        {
                            _infoBlock.gameObject.SetActive(false);
                            _btnImage.fillAmount = 1;
                        });
                }

                ViewModel.ShowInfoWave.OnNext(false);
            }).AddTo(ref d);
            ViewModel.PositionInfoBtn.Subscribe(p => _infoBlock.transform.position = p).AddTo(ref d);
            ViewModel.FillAmountBtn.Subscribe(n => _btnImage.fillAmount = n).AddTo(ref d);

            ViewModel.ShowInfoWave.Subscribe(showInfo =>
            {
                if (showInfo)
                {
                    _infoPanel.gameObject.SetActive(true);
                    _infoPanel.DOScale(1, 0.3f)
                        .From(0.3f)
                        .SetEase(Ease.OutBack)
                        .SetUpdate(true);
                }
                else
                {
                    _infoPanel.DOScale(0f, 0.3f)
                        .From(1)
                        .SetEase(Ease.InBack)
                        .SetUpdate(true)
                        .OnComplete(() => _infoPanel.gameObject.SetActive(false));
                    
                }
            }).AddTo(ref d);
            InfoWavePanelClear();
            foreach (var defenceCountMob in ViewModel.InfoWaveMobs)
            {
                InfoWavePanelAddEntity(defenceCountMob);
            }

            ViewModel.InfoWaveMobs.ObserveAdd()
                .Subscribe(e => InfoWavePanelAddEntity(e.Value)).
                AddTo(ref d);
            ViewModel.InfoWaveMobs.ObserveClear().Subscribe(_ => InfoWavePanelClear()).AddTo(ref d);

            //Блок инфо о башне
            ViewModel.ShowInfoTower.Subscribe(showTower =>
            {
                if (showTower)
                {
                    //TODO Заполняем данне о башне
                    //MobDefence
                    //Эпичность
                    //Название
                    //Уровень в Геймплее
                    //Debug.Log("ShowInfoTower " + showTower);
                    if (_infoTower.gameObject.activeSelf)
                    {
                        _infoTower.localScale = Vector3.one * 0.3f;
                    }
                    _infoTower.gameObject.SetActive(true);
                    _infoTower.DOScale(1, 0.3f)
                        .From(0.3f)
                        .SetEase(Ease.OutBack)
                        .SetUpdate(true);
                }
                else
                {
                    //Debug.Log("ShowInfoTower " + showTower);
                    _infoTower.DOScale(0f, 0.3f)
                        .From(1)
                        .SetEase(Ease.InBack)
                        .SetUpdate(true)
                        .OnComplete(() => _infoTower.gameObject.SetActive(false));
                }
            }).AddTo(ref d);

            ViewModel.PositionInfoTower.Subscribe(p => _infoTower.transform.position = p).AddTo(ref d);
            ViewModel.BaseParameters.ObserveAdd().Subscribe(e => InfoTowerBaseParametersAddEntity(e.Value)).AddTo(ref d);
            ViewModel.BaseParameters.ObserveClear().Subscribe(_ => InfoTowerBaseParametersClear()).AddTo(ref d);
            ViewModel.UpgradeParameters.ObserveAdd().Subscribe(e => InfoTowerUpgradeParametersAddEntity(e.Value)).AddTo(ref d);
            ViewModel.UpgradeParameters.ObserveClear().Subscribe(_ => InfoTowerUpgradeParametersClear()).AddTo(ref d);

            _disposableImplementation = d.Build();
        }

        private void InfoWavePanelAddEntity(KeyValuePair<string, int> objValue)
        {
            var count = _enemyInfoBinders.Count;
            var configId = objValue.Key;
            
            var prefabPath = $"Prefabs/UI/Gameplay/Panels/GateWaveInfo/EnemyInfo"; //Перенести в настройки уровня
            var enemyPrefab = Resources.Load<EnemyInfoBinder>(prefabPath);
            var createdInfoString = Instantiate(enemyPrefab, _enemies);
            
            var mobConfig = ViewModel.MobsSettings.AllMobs.Find(t => t.ConfigId == configId);
            if (mobConfig == null) 
                mobConfig = ViewModel.MobsSettings.AllBosses.Find(t => t.ConfigId == configId);
            
            Sprite image = _imageManager.GetDefence(mobConfig.Defence);

            createdInfoString.Bind(
                image,
                mobConfig.TitleLid,
                objValue.Value,
                count
            );
            _enemyInfoBinders.Add(createdInfoString);
            _enemies.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 310 + 110 * count);
        }

        private void InfoWavePanelClear()
        {
            foreach (var infoBinder in _enemyInfoBinders)
            {
                Destroy(infoBinder.gameObject);
            }

            _enemyInfoBinders.Clear();
            _enemies.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 270);
        }

        private void InfoTowerBaseParametersAddEntity(KeyValuePair<TowerParameterType, float> objValue)
        {
            var count = _baseParameterBinders.Count;
            var prefabPath = $"Prefabs/UI/Gameplay/Panels/GateWaveInfo/BaseParameter"; //Перенести в настройки уровня
            var paramPrefab = Resources.Load<BaseParameterBinder>(prefabPath);
            var createdBinder = Instantiate(paramPrefab, _baseParameters);
            
            
            Sprite image = _imageManager.GetParameter(objValue.Key);
   
            //if (image == null) throw new Exception("Не найдено изображение для " + objValue.Key);
            createdBinder.Bind(image, objValue.Key.GetString(), objValue.Value, count);
            _baseParameterBinders.Add(createdBinder);
            _baseParameters.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 320 + 105 * count);
        }

        private void InfoTowerBaseParametersClear()
        {
            foreach (var infoBinder in _baseParameterBinders)
            {
                Destroy(infoBinder.gameObject);
            }
            _baseParameterBinders.Clear();
            
            //_enemies.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 270);
        }

        private void InfoTowerUpgradeParametersAddEntity(KeyValuePair<TowerParameterType, float> objValue)
        {
            //TODO Загружаем из префаба
        }

        private void InfoTowerUpgradeParametersClear()
        {
            foreach (var infoBinder in _upgradeParameterBinders)
            {
                Destroy(infoBinder.gameObject);
            }
            _upgradeParameterBinders.Clear();
        }


        private void OnEnable()
        {
            _btnInfo.onClick.AddListener(OnStartForced); 
        }

        private void OnDisable()
        {
            _btnInfo.onClick.RemoveListener(OnStartForced);
        }

        private void OnStartForced()
        {
            if (!ViewModel.ShowInfoWave.CurrentValue)
            {
                ViewModel.ShowInfoWave.OnNext(true);
            }
            else
            {
                ViewModel.StartForcedWave();
            }
        }

        public override void Show()
        {
        }

        public override void Hide()
        {
        }

        public void OnDestroy()
        {
            _disposableImplementation.Dispose();
        }
    }
}