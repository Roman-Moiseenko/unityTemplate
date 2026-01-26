using System;
using System.Collections.Generic;
using Game.GamePlay.View.UI.ScreenGameplay.Popups;
using Game.GamePlay.View.UI.ScreenGameplay.Rewards;
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

        //[SerializeField] private TMP_Text _textProgress;
        [SerializeField] private TMP_Text _textMoney;
        [SerializeField] private TMP_Text _textCrystal;
        [SerializeField] private TMP_Text _textWave;

        [SerializeField] private Transform _panelPopupMessages;
        [SerializeField] private Transform _panelRewardPopup;
        [SerializeField] private List<DamagePopupBinder> _damagePopups;
        [SerializeField] private List<CurrencyPopupBinder> _currencyPopups;
        [SerializeField] private List<CurrencyPopupBinder> _progressPopups;
        [SerializeField] private RewardEntityBinder rewardEntity;

        [SerializeField] private Transform _targetCurrency;
        [SerializeField] private Transform _targetProgress;
        [SerializeField] private ReducePopupBinder _reducePopupBinder;
        [SerializeField] private CastleHealthBarBinder _castleHealthBar;
        [SerializeField] private Transform _progressContainer;

        [SerializeField] private Transform startWave;
        [SerializeField] private Transform finishWave;

        [SerializeField] private Transform topMenu;
        
        private TMP_Text _levelProgress;
        private Slider _progress;
        private IDisposable _disposable;

        //TODO РАЗБИТЬ НА МАЛЫЕ ЗАДАЧИ В ОТДЕЛЬНЫЕ BINDER
        //1. TopMenu
        //2. Блок показа начала и конца волны
        //3. Панель для Сообщений
        //4. Панель для Наград
        
        
        private void Awake()
        {
            _levelProgress = _progressContainer.Find("Level").GetComponent<TMP_Text>();
            _progress = _progressContainer.Find("Slider").GetComponent<Slider>();
        }

        protected override void OnBind(ScreenGameplayViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();

            startWave.gameObject.SetActive(false);
            finishWave.gameObject.SetActive(false);
            rewardEntity.Bind();
            
            //Пул всплывающих popup
            for (var i = 0; i < 20; i++)
            {
                CreateDamagePopup();
            }

            //Пул всплывающих монеток
            for (int i = 0; i < 10; i++)
            {
                CreateCurrencyPopup();
            }

            //Пул всплывающих кристалов
            for (int i = 0; i < 10; i++)
            {
                CreateProgressPopup();
            }

            ViewModel.ProgressData.Subscribe(newValue =>
            {
                var p = newValue / 100f;
                _progress.value = p > 1 ? 1 : p;
            }).AddTo(ref d);
            ViewModel.ProgressLevel.Skip(1).Subscribe(newValue => { _levelProgress.text = newValue.ToString(); })
                .AddTo(ref d);

            //ViewModel.ProgressData.Subscribe(newValue => { _textProgress.text = $"Progress: {newValue}"; });
            ViewModel.SoftCurrency.Subscribe(newValue => _textMoney.text = newValue.ToString()).AddTo(ref d);
            ViewModel.HardCurrency.Subscribe(newValue => _textCrystal.text = newValue.ToString()).AddTo(ref d);
            ViewModel.WaveText.Subscribe(newValue => _textWave.text = newValue).AddTo(ref d);
            _reducePopupBinder = CreateReducePopup();
            _castleHealthBar = CreateCastleHealthBar();
            ViewModel.AllDamages.ObserveAdd().Subscribe(e =>
            {
                var damageData = e.Value;
                var popup = FindFreePopup(); //Найти первое свободное окно
                var position = new Vector3(damageData.Position.x, 1f, damageData.Position.y);
                popup.StartPopup(position, damageData.Damage, damageData.Type);
                ViewModel.AllDamages.Remove(e.Value); // Сразу удаляем
            }).AddTo(ref d);

            ViewModel.RepairBuffer.ObserveAdd().Subscribe(e =>
            {
                //Показываем восстановление
                var position = new Vector3(0, 1f, 0);
                _reducePopupBinder.StartPopup(position, e.Value);
                ViewModel.RepairBuffer.Remove(e.Value);
            }).AddTo(ref d);

            ViewModel.CastleHealth
                .Skip(1)
                .Subscribe(h => _castleHealthBar.SetHealth(h))
                .AddTo(ref d);

            ViewModel.AllRewards.ObserveAdd().Subscribe(r =>
            {
                var currency = FindFreeCurrency();
                var progress = FindFreeProgress();
                var position = new Vector3(r.Value.Position.x, 1f, r.Value.Position.y);
                currency.StartPopup(position);
                progress.StartPopup(position);
                currency.Free.Skip(1).Subscribe(v =>
                {
                    //При освобождении удаляем из списка
                    if (v) ViewModel.AllRewards.Remove(r.Value);
                });
            }).AddTo(ref d);

            ViewModel.RewardEntity.Where(r => r != null).Subscribe(r =>
            {
                var position = new Vector3(r.Position.x, 1f, r.Position.y);
                position = ViewModel.CameraService.Camera.WorldToScreenPoint(position);
                rewardEntity.StartPopup(r.RewardType, r.ConfigId, position);

            }).AddTo(ref d);
            //Показываем инфо начала и окончания волны

            ViewModel.ShowStartWave.Where(show => show).Subscribe(_ =>
            {
                startWave.gameObject.SetActive(true);
                ViewModel.ShowStartWave.Value = false;
            }).AddTo(ref d);
            ViewModel.ShowFinishWave.Where(show => show).Subscribe(_ =>
            {
                finishWave.gameObject.SetActive(true);
                ViewModel.ShowFinishWave.Value = false;
            }).AddTo(ref d);

            ViewModel.ShowTopMenu.Subscribe(v => topMenu.gameObject.SetActive(v)).AddTo(ref d);
            _disposable = d.Build();
        }

        private DamagePopupBinder FindFreePopup()
        {
            foreach (var popupBinder in _damagePopups)
            {
                if (popupBinder.Free.Value) return popupBinder;
            }

            return CreateDamagePopup(); //Расширяем пул всплывающих popup на 1
        }

        private CurrencyPopupBinder FindFreeCurrency()
        {
            foreach (var popupBinder in _currencyPopups)
            {
                if (popupBinder.Free.Value) return popupBinder;
            }

            return CreateCurrencyPopup();
        }

        private CurrencyPopupBinder FindFreeProgress()
        {
            foreach (var popupBinder in _progressPopups)
            {
                if (popupBinder.Free.Value) return popupBinder;
            }

            return CreateProgressPopup();
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
            _disposable.Dispose();
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

        private DamagePopupBinder CreateDamagePopup()
        {
            var prefabPath = "Prefabs/UI/Gameplay/ScreenGameplay/DamagePopup";
            var damagePopupPrefab = Resources.Load<DamagePopupBinder>(prefabPath);
            var createdDamagePopup = Instantiate(damagePopupPrefab, _panelPopupMessages);
            createdDamagePopup.Bind(ViewModel.CameraService.Camera, ViewModel.PositionCamera);
            _damagePopups.Add(createdDamagePopup);
            return createdDamagePopup;
        }

        private ReducePopupBinder CreateReducePopup()
        {
            var prefabPath = "Prefabs/UI/Gameplay/ScreenGameplay/ReduceHealthPopup";
            var reducePopupPrefab = Resources.Load<ReducePopupBinder>(prefabPath);
            var createdReducePopup = Instantiate(reducePopupPrefab, _panelPopupMessages);
            createdReducePopup.Bind(ViewModel.CameraService.Camera, ViewModel.PositionCamera);

            return createdReducePopup;
        }

        private CastleHealthBarBinder CreateCastleHealthBar()
        {
            var prefabPath = "Prefabs/UI/Gameplay/ScreenGameplay/CastleHealthBar";
            var castleHealthPrefab = Resources.Load<CastleHealthBarBinder>(prefabPath);
            var createdCastleHealth = Instantiate(castleHealthPrefab, _panelPopupMessages);
            createdCastleHealth.Bind(ViewModel.CameraService.Camera, ViewModel.PositionCamera,
                ViewModel.CastleFullHealth);

            return createdCastleHealth;
        }

        private CurrencyPopupBinder CreateCurrencyPopup()
        {
            var prefabPath = "Prefabs/UI/Gameplay/ScreenGameplay/RewardCurrency";
            var currencyPopupPrefab = Resources.Load<CurrencyPopupBinder>(prefabPath);
            var createdCurrencyPopup = Instantiate(currencyPopupPrefab, _panelRewardPopup);
            createdCurrencyPopup.Bind(ViewModel.CameraService.Camera, ViewModel.PositionCamera,
                _targetCurrency.transform.position);
            _currencyPopups.Add(createdCurrencyPopup);
            return createdCurrencyPopup;
        }

        private CurrencyPopupBinder CreateProgressPopup()
        {
            var prefabPath = "Prefabs/UI/Gameplay/ScreenGameplay/RewardProgress";
            var progressPopupPrefab = Resources.Load<CurrencyPopupBinder>(prefabPath);
            var createdProgressPopup = Instantiate(progressPopupPrefab, _panelRewardPopup);
            createdProgressPopup.Bind(ViewModel.CameraService.Camera, ViewModel.PositionCamera,
                _targetProgress.transform.position);
            _progressPopups.Add(createdProgressPopup);
            return createdProgressPopup;
        }
    }
}