using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Common;
using Game.GameRoot.ImageManager;
using Game.State.Inventory;
using Game.State.Maps.Towers;
using ObservableCollections;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelGateWave.InfoTower
{
    public class InfoTowerBinder : MonoBehaviour
    {
        [SerializeField] private Transform baseParameters;
        [SerializeField] private Transform boosters;
        
        [SerializeField] private Image header;
        [SerializeField] private Image output;
        [SerializeField] private Image background;
        [SerializeField] private TMP_Text nameEpic;
        [SerializeField] private TMP_Text nameTower;

        [SerializeField] private List<BaseParameterBinder> parameterBinders;
        [SerializeField] private List<BoosterParameterBinder> boosterBinders;
        
        [SerializeField] private Image backDefence;
        [SerializeField] private Image iconDefence;
        
        [SerializeField] private Transform stars;
        
        private InfoTowerViewModel _viewModel;
        private IDisposable _disposable;
        private ImageManagerBinder _imageManager;
        public void Bind(InfoTowerViewModel viewModel)
        {
            _viewModel = viewModel;
            transform.gameObject.SetActive(false);
            _imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            
            var d = Disposable.CreateBuilder();
            viewModel.ShowInfoTower.Subscribe(showTower =>
            {
                if (showTower)
                {
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
            //Перед показываением окна, обновляем его содержимое
            viewModel.UpdateInfoBackgroundTower.Where(x => x).Subscribe(_ =>
            {
                //Фон от эпичности 
                var epicImage = _imageManager.GetEpicData(viewModel.EpicLevel);
                header.sprite = epicImage.Header;
                output.sprite = epicImage.Output;
                background.sprite = epicImage.Background;

                nameEpic.text = viewModel.EpicLevel.GetString();
                nameTower.text = viewModel.NameTower;
                
                var defenceImage = _imageManager.GetDefenceData(viewModel.Defence);
                backDefence.sprite = defenceImage.Background;
                iconDefence.sprite = defenceImage.Icon;
                
                //Звездочки
                for (var i = 1; i <= 6; i++)
                {
                    var star = stars.Find($"Container/Star{i}").GetComponent<Transform>();
                    star.gameObject.SetActive(i <= viewModel.Level);
                }
                //Эффекты
                InfoTowerBoosters();
                //Параметры башни
                InfoTowerParameters();
                
            }).AddTo(ref d);

            viewModel.PositionInfoTower.Subscribe(p => transform.transform.position = p).AddTo(ref d);

            _disposable = d.Build();
        }
        
        private void InfoTowerBoosters()
        {
            if (_viewModel.BoosterParameters.Count == 0)
            {
                boosters.gameObject.SetActive(false);
                return;
            }
            boosters.gameObject.SetActive(true);
            
            foreach (var boosterBinder in boosterBinders)
                boosterBinder.gameObject.SetActive(false);

            var index = 0;
            
            foreach (var (parameter, value) in _viewModel.BoosterParameters)
            {
                var image = _imageManager.GetParameter(parameter);
                boosterBinders[index].Bind(image, value);
                boosterBinders[index].gameObject.SetActive(true);
                index++;
            }
            
        }

        public void InfoTowerParameters()
        {
            foreach (var parameterBinder in parameterBinders)
                parameterBinder.gameObject.SetActive(false);

            var index = 0;

            foreach (var (parameter, value) in _viewModel.BaseParameters)
            {
                var image = _imageManager.GetParameter(parameter);
                
                parameterBinders[index].Bind(image, parameter.GetString(), value, 0, false);
                parameterBinders[index].gameObject.SetActive(true);
                index++;
            }
        }


   



        public void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}