using System;
using System.Collections;
using System.Collections.Generic;
using Game.Common;
using Game.GamePlay.View.Skills;
using Game.GameRoot.ImageManager;
using R3;
using TMPro;
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
        [SerializeField] private TMP_Text txtCooldown;
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
                    star.Find("starFill").gameObject.SetActive(index <= level);
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
            imageCooldown.gameObject.SetActive(false);
            txtCooldown.gameObject.SetActive(false);
            viewModel.IsCooldown.Where(x => x).Subscribe(v =>
            {
                imageCooldown.fillAmount = 1f;
                txtCooldown.text = Mathf.RoundToInt(_viewModel.Cooldown).ToString();
                
                imageCooldown.gameObject.SetActive(true);
                txtCooldown.gameObject.SetActive(true);
                StartCoroutine(TimeOut());
                //Запуск корутины таймера
            }).AddTo(ref d);

            _disposable = d.Build();
        }

        private IEnumerator TimeOut()
        {
            yield return new WaitForSeconds(1f);
            _viewModel.TimeOut--;
            imageCooldown.fillAmount = _viewModel.TimeOut / _viewModel.Cooldown;
            txtCooldown.text = Mathf.RoundToInt(_viewModel.TimeOut).ToString();
            if (_viewModel.TimeOut > 0)
            {
                StartCoroutine(TimeOut());
            }
            else
            {
                _viewModel.IsCooldown.Value = false;
                imageCooldown.gameObject.SetActive(false);
                txtCooldown.gameObject.SetActive(false);
            }
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
            _viewModel.StartSkill();
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}