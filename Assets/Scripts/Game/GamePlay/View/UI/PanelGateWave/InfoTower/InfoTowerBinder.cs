using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Common;
using Game.GameRoot.ImageManager;
using Game.State.Maps.Towers;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelGateWave.InfoTower
{
    public class InfoTowerBinder : MonoBehaviour
    {
        [SerializeField] private Transform baseParameters;
        [SerializeField] private Transform upgradeParameters;

        [SerializeField] private Image header;
        
        
        private readonly List<BaseParameterBinder> _baseParameterBinders = new();
        private readonly List<UpgradeParameterBinder> _upgradeParameterBinders = new();
        
        private InfoTowerViewModel _viewModel;
        private IDisposable _disposable;
        private ImageManagerBinder _imageManager;
        public void Bind(InfoTowerViewModel viewModel)
        {
            _viewModel = viewModel;
            transform.gameObject.SetActive(false);
            _imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            
            //Загружаем фон из _imageManager
            //Меняем название и эпичность
            
            
            
            
            var d = Disposable.CreateBuilder();
            viewModel.ShowInfoTower.Subscribe(showTower =>
            {
                if (showTower)
                {
                    //TODO Заполняем данне о башне
                    //MobDefence
                    //Эпичность
                    //Название
                    //Уровень в Геймплее
                    //Debug.Log("ShowInfoTower " + showTower);
                    if (transform.gameObject.activeSelf)
                    {
                        transform.localScale = Vector3.one * 0.3f;
                    }
                    transform.gameObject.SetActive(true);
                    transform.DOScale(1, 0.3f)
                        .From(0.3f)
                        .SetEase(Ease.OutBack)
                        .SetUpdate(true)
                        .OnComplete(() => transform.gameObject.SetActive(true));
                }
                else
                {
                    transform.DOScale(0f, 0.3f)
                        .From(1)
                        .SetEase(Ease.InBack)
                        .SetUpdate(true)
                        .OnComplete(() => transform.gameObject.SetActive(false));
                }
            }).AddTo(ref d);
            viewModel.UpdateInfoBackgroundTower.Where(x => x).Subscribe(_ =>
            {
                Debug.Log("UpdateInfoBackgroundTower");
                Debug.Log(viewModel.NameTower);
                Debug.Log(viewModel.Level);
                Debug.Log(viewModel.EpicLevel);
                Debug.Log(viewModel.Defence);
                
            }).AddTo(ref d);

            viewModel.PositionInfoTower.Subscribe(p => transform.transform.position = p).AddTo(ref d);
            viewModel.BaseParameters.ObserveAdd().Subscribe(e => InfoTowerBaseParametersAddEntity(e.Value)).AddTo(ref d);
            viewModel.BaseParameters.ObserveClear().Subscribe(_ => InfoTowerBaseParametersClear()).AddTo(ref d);
            viewModel.UpgradeParameters.ObserveAdd().Subscribe(e => InfoTowerUpgradeParametersAddEntity(e.Value)).AddTo(ref d);
            viewModel.UpgradeParameters.ObserveClear().Subscribe(_ => InfoTowerUpgradeParametersClear()).AddTo(ref d);

            _disposable = d.Build();
        }
        private void InfoTowerBaseParametersAddEntity(KeyValuePair<TowerParameterType, float> objValue)
        {
            var count = _baseParameterBinders.Count;
            var prefabPath = $"Prefabs/UI/Gameplay/Panels/GateWaveInfo/BaseParameter"; //Перенести в настройки уровня
            var paramPrefab = Resources.Load<BaseParameterBinder>(prefabPath);
            var createdBinder = Instantiate(paramPrefab, baseParameters);
            
            
            var image = _imageManager.GetParameter(objValue.Key);
   
            //if (image == null) throw new Exception("Не найдено изображение для " + objValue.Key);
            createdBinder.Bind(image, objValue.Key.GetString(), objValue.Value, count);
            _baseParameterBinders.Add(createdBinder);
            baseParameters.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 320 + 105 * count);
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
        public void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}