using System;
using System.Collections;
using System.Collections.Generic;
using Game.Common;
using Game.GamePlay.View.Skills;
using Game.GameRoot.ImageManager;
using R3;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelActions
{
    public class ButtonSkillBinder : MonoBehaviour
    {
        [SerializeField] public Button startButton;
        [SerializeField] private Transform selectedBackground;
        [SerializeField] private Transform selectedIcon;
        [SerializeField] private Image imageEpicBackground;
        [SerializeField] private Image imageSkill;
        [SerializeField] private Image imageCooldown;
        [SerializeField] private List<Transform> stars;
        private SkillViewModel _viewModel;
        private ImageManagerBinder _imageManager;
        private IDisposable _disposable;
        
        private void Awake()
        {
            _imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
        }

        //TODO Передать колбек для вызова функции по нажатию и данные по скиллу
        public void Bind(SkillViewModel viewModel)
        {
            
            _viewModel = viewModel;
            imageEpicBackground.sprite = _imageManager.GetEpicSkillLevel(viewModel.EpicLevel);
            imageSkill.sprite = _imageManager.GetSkillCard(viewModel.ConfigId);

            var d = Disposable.CreateBuilder();
            viewModel.Level.Subscribe(level =>
            {
                var index = 0;
                foreach (var star in stars)
                {
                    index++;
                    if (index <= level)
                    {
                        star.Find("starFill").gameObject.SetActive(true);
                    }
                    else
                    {
                        star.Find("starFill").gameObject.SetActive(false);
                    }
                }
            }).AddTo(ref d);
            //selected.Find("Icon").transform.SetAsLastSibling();
            selectedIcon.SetAsLastSibling();
            viewModel.IsActive.Subscribe(v =>
            {
   //             Debug.Log(viewModel.ConfigId + " " + v);
                selectedBackground.gameObject.SetActive(v);
                selectedIcon.gameObject.SetActive(v);
            }).AddTo(ref d);
            viewModel.TimeOut.Subscribe(v =>
            {
                if (v == viewModel.Cooldown)
                {
                    imageCooldown.fillAmount = 0f;
                    imageCooldown.gameObject.SetActive(true);
                    //Запуск корутины таймера
                    return;
                }
                imageCooldown.fillAmount = _viewModel.TimeOut.Value / _viewModel.Cooldown;
                if (v == 0)
                {
                    imageCooldown.gameObject.SetActive(false);
                    //Остановка 
                    return;
                }
                
            }).AddTo(ref d);
//            SelectedToggle();
            
            _disposable = d.Build();
        }

        private IEnumerator TimeOut()
        {
            yield return new WaitForSeconds(1f);
            _viewModel.TimeOut.Value--;
        }
        
        private void SelectedToggle()
        {
            selectedBackground.gameObject.SetActive(!selectedBackground.gameObject.activeSelf);
            selectedIcon.gameObject.SetActive(!selectedIcon.gameObject.activeSelf);
        }

        private void OnEnable()
        {
            startButton.onClick.AddListener(OnStartSkill);
        }

        private void OnDisable()
        {
            startButton.onClick.AddListener(OnStartSkill);
        }

        private void OnStartSkill()
        {
            if (!_viewModel.IsEnabled.CurrentValue) return;
            _viewModel.StartSkill();
            //SelectedToggle();
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}