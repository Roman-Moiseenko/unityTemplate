using System;
using System.Collections.Generic;
using Game.GamePlay.View.Damages;
using MVVM.UI;
using ObservableCollections;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Game.GamePlay.View.UI.ScreenGameplay
{
    public class ScreenGameplayBinder : WindowBinder<ScreenGameplayViewModel>
    {
        [SerializeField] private Button _btnPopupPause;
        [SerializeField] private TMP_Text _textProgress;
        [SerializeField] private TMP_Text _textMoney;
        [SerializeField] private TMP_Text _textCrystal;
        [SerializeField] private TMP_Text _textWave;

        [SerializeField] private Canvas _damagePanel;
        [SerializeField] private List<DamagePopupBinder> _damagePopups;

        private void Start()
        {
            for (int i = 0; i < 20; i++) //Пул всплывающих popup
            {
                AddDamagePopup();
            }
            
            ViewModel.ProgressData.Subscribe(newValue => { _textProgress.text = $"Progress: {newValue}"; });
            ViewModel.SoftCurrency.Subscribe(newValue => _textMoney.text = $"Money: {newValue}");
            ViewModel.HardCurrency.Subscribe(newValue => _textCrystal.text = $"Money: {newValue}");
            ViewModel.WaveText.Subscribe(newValue => _textWave.text = newValue);
            
            ViewModel.AllDamages.ObserveAdd().Subscribe(e =>
            {
                var damageData = e.Value;
                var popup = FindFreePopup(); //Найти первое свободное окно
                var position = new Vector3(damageData.Position.x, 1f, damageData.Position.y);
                popup.StartPopup(position, damageData.Damage, damageData.Type);
                ViewModel.AllDamages.Remove(e.Value); // Сразу удаляем
            });
        }

        private DamagePopupBinder FindFreePopup()
        {
            foreach (var popupBinder in _damagePopups)
            {
                if (popupBinder.Free.Value) return popupBinder;
            }
            return AddDamagePopup(); //Расширяем пул всплывающих popup на 1
        }

        private void OnEnable()
        {
            _btnPopupPause.onClick.AddListener(OnPopupPauseButtonClicked);
        }

        private void OnDisable()
        {
            _btnPopupPause.onClick.RemoveListener(OnPopupPauseButtonClicked);
        }

        private void OnDestroy()
        {
            foreach (var binder in _damagePopups)
            {
                Destroy(binder.gameObject);
            }
        }

        private void OnPopupBButtonClicked()
        {
            ViewModel.RequestOpenPopupB();
        }

        private void OnPopupPauseButtonClicked()
        {
            ViewModel.RequestOpenPopupPause();
        }

        private DamagePopupBinder AddDamagePopup()
        {
            var prefabPath = "Prefabs/UI/Gameplay/DamagePopup"; //Перенести в настройки уровня
            var damagePopupPrefab = Resources.Load<DamagePopupBinder>(prefabPath);
            var createdDamagePopup = Instantiate(damagePopupPrefab, _damagePanel.transform);
            createdDamagePopup.Bind(ViewModel.CameraService.Camera, ViewModel.PositionCamera);
            _damagePopups.Add(createdDamagePopup);

            return createdDamagePopup;
        }
    }
}