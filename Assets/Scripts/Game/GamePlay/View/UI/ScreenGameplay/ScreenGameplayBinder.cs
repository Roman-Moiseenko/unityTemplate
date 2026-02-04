using System;
using System.Collections.Generic;
using Game.GamePlay.View.UI.ScreenGameplay.Popups;
using Game.GamePlay.View.UI.ScreenGameplay.Rewards;
using MVVM.Storage;
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

        private PoolMono<DamagePopupBinder> _poolDamages;//Пул всплывающих popup
        private PoolMono<CurrencyPopupBinder> _poolCurrency;//Пул всплывающих монеток
        private PoolMono<CurrencyPopupBinder> _poolProgress;//Пул всплывающих кристалов

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
            
            //Инициализируем пулы объектов
            _poolDamages = new PoolMono<DamagePopupBinder>(
                "Prefabs/UI/Gameplay/ScreenGameplay/DamagePopup", 
                20, 
                _panelPopupMessages);
            _poolCurrency = new PoolMono<CurrencyPopupBinder>(
                "Prefabs/UI/Gameplay/ScreenGameplay/RewardCurrency", 
                7, 
                _panelPopupMessages);
            _poolProgress = new PoolMono<CurrencyPopupBinder>(
                "Prefabs/UI/Gameplay/ScreenGameplay/RewardProgress", 
                7, 
                _panelPopupMessages);
            
            
            ViewModel.ProgressData.Subscribe(newValue =>
            {
                var p = newValue / 100f;
                _progress.value = p > 1 ? 1 : p;
            }).AddTo(ref d);
            ViewModel.ProgressLevel.Skip(1).Subscribe(newValue => { _levelProgress.text = newValue.ToString(); })
                .AddTo(ref d);

            ViewModel.SoftCurrency.Subscribe(newValue => _textMoney.text = newValue.ToString()).AddTo(ref d);
            ViewModel.HardCurrency.Subscribe(newValue => _textCrystal.text = newValue.ToString()).AddTo(ref d);
            ViewModel.WaveText.Subscribe(newValue => _textWave.text = newValue).AddTo(ref d);
            _reducePopupBinder = CreateReducePopup();
            _castleHealthBar = CreateCastleHealthBar();
            ViewModel.AllDamages.ObserveAdd().Subscribe(e =>
            {
                var damageData = e.Value;
                var popup = _poolDamages.GetFreeElement();
                var position = new Vector3(damageData.Position.x, 1f, damageData.Position.y);
                popup.StartPopup(position, damageData.Damage, damageData.Type, ViewModel.PositionCamera);
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
                var currency = _poolCurrency.GetFreeElement();
                var progress = _poolProgress.GetFreeElement();
                var position = new Vector3(r.Value.Position.x, 1f, r.Value.Position.y);
                
                //На одной из наград отслеживает, когда доберется до конца
                currency.StartPopup(position, _targetProgress.position)
                    .Where(x => x)
                    .Subscribe(v => ViewModel.AllRewards.Remove(r.Value));
                
                progress.StartPopup(position, _targetProgress.position);

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

        }

        private void OnPopupPauseButtonClicked()
        {
            ViewModel.RequestOpenPopupPause();
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
        
    }
}