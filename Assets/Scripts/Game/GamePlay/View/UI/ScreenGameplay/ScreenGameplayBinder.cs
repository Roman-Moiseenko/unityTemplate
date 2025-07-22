using System;
using System.Collections.Generic;
using Game.GamePlay.View.UI.ScreenGameplay.Popups;
using MVVM.UI;
using ObservableCollections;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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

        [SerializeField] private Canvas _panelPopupMessages;
        [SerializeField] private List<DamagePopupBinder> _damagePopups;
        [SerializeField] private ReducePopupBinder _reducePopupBinder;
        [SerializeField] private CastleHealthBarBinder _castleHealthBar;

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
            _reducePopupBinder = createReducePopup();
            _castleHealthBar = createCastleHealthBar();
            ViewModel.AllDamages.ObserveAdd().Subscribe(e =>
            {
                var damageData = e.Value;
                var popup = FindFreePopup(); //Найти первое свободное окно
                var position = new Vector3(damageData.Position.x, 1f, damageData.Position.y);
                popup.StartPopup(position, damageData.Damage, damageData.Type);
                ViewModel.AllDamages.Remove(e.Value); // Сразу удаляем
            });

            ViewModel.RepairBuffer.ObserveAdd().Subscribe(e =>
            {
                //TODO Показываем восстановление
                var position = new Vector3(0, 1f, 0);
                _reducePopupBinder.StartPopup(position, e.Value);
                ViewModel.RepairBuffer.Remove(e.Value);
            });

            ViewModel.CastleHealth.Skip(1).Subscribe(h => _castleHealthBar.SetHealth(h));
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
            var prefabPath = "Prefabs/UI/Gameplay/ScreenGameplay/DamagePopup";
            var damagePopupPrefab = Resources.Load<DamagePopupBinder>(prefabPath);
            var createdDamagePopup = Instantiate(damagePopupPrefab, _panelPopupMessages.transform);
            createdDamagePopup.Bind(ViewModel.CameraService.Camera, ViewModel.PositionCamera);
            _damagePopups.Add(createdDamagePopup);
            return createdDamagePopup;
        }

        private ReducePopupBinder createReducePopup()
        {
            var prefabPath = "Prefabs/UI/Gameplay/ScreenGameplay/ReduceHealthPopup";
            var reducePopupPrefab = Resources.Load<ReducePopupBinder>(prefabPath);
            var createdReducePopup = Instantiate(reducePopupPrefab, _panelPopupMessages.transform);
            createdReducePopup.Bind(ViewModel.CameraService.Camera, ViewModel.PositionCamera);

            return createdReducePopup;
        }

        private CastleHealthBarBinder createCastleHealthBar()
        {
            var prefabPath = "Prefabs/UI/Gameplay/ScreenGameplay/CastleHealthBar";
            var castleHealthPrefab = Resources.Load<CastleHealthBarBinder>(prefabPath);
            var createdCastleHealth = Instantiate(castleHealthPrefab, _panelPopupMessages.transform);
            createdCastleHealth.Bind(ViewModel.CameraService.Camera, ViewModel.PositionCamera,
                ViewModel.CastleFullHealth);

            return createdCastleHealth;
        }
    }
}