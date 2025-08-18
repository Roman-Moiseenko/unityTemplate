using System.Collections.Generic;
using Game.State.Maps.Mobs;
using MVVM.UI;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelGateWave
{
    public class PanelGateWaveBinder : PanelBinder<PanelGateWaveViewModel>
    {
        [SerializeField] private Button _btnInfo;
        [SerializeField] private Transform _infoBlock;
        [SerializeField] private Transform _infoPanel;
        [SerializeField] private Transform enemies;

        private List<EnemyInfoBinder> _enemyInfoBinders = new();
        
        
       // [SerializeField] private Button _btnCloseInfo;
        
        private Image _btnImage;
        private Animator _animator;
        
        private void Start()
        {
            _btnImage = _btnInfo.GetComponent<Image>();
            _btnImage.fillAmount = 1;
            _animator = gameObject.GetComponent<Animator>();
            ViewModel.IsSelected.OnNext(false);
            ViewModel.ShowInfoWave.Subscribe(showButton =>
            {
                if (showButton)
                {
                    _infoBlock.gameObject.SetActive(true);  
                    _animator.Play("info_wave_start");
                }
                else
                {
                    _animator.Play("info_wave_finish");
                }
                
                
                ViewModel.IsSelected.OnNext(false);
            });
            ViewModel.PositionInfoBtn.Subscribe(p =>
            {
                _infoBlock.transform.position = p;
            });
            ViewModel.FillAmountBtn.Subscribe(n =>
            {
                _btnImage.fillAmount = n;
            });

            ViewModel.IsSelected.Subscribe(showInfo =>
            {
                if (showInfo)
                {
                    _infoPanel.gameObject.SetActive(true);  
                    //_animator.Play("info_panel_start");
                }
                else
                {
                    _infoPanel.gameObject.SetActive(false);  
                    //_animator.Play("info_panel_finish");
                }
            });
            InfoPanelClear();
            foreach (var defenceCountMob in ViewModel.DefenceCountMobs)
            {
                InfoPanelAddEntity(defenceCountMob);
            }
            ViewModel.DefenceCountMobs.ObserveAdd().Subscribe(e => InfoPanelAddEntity(e.Value));

            ViewModel.DefenceCountMobs.ObserveClear().Subscribe(_ => InfoPanelClear());
        }

        private void InfoPanelAddEntity(KeyValuePair<MobType, int> objValue)
        {
            var count = _enemyInfoBinders.Count;
            var prefabPath = $"Prefabs/UI/Gameplay/Panels/GateWaveInfo/EnemyInfo"; //Перенести в настройки уровня
            var enemyPrefab = Resources.Load<EnemyInfoBinder>(prefabPath);
            var createdInfoString = Instantiate(enemyPrefab, enemies);
            createdInfoString.Bind(
                objValue.Key.GetDefence(), 
                objValue.Key.GetString(), 
                objValue.Value, 
                count
                );
            _enemyInfoBinders.Add(createdInfoString);
            enemies.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 270 + 155 * count);
        }

        private void InfoPanelClear()
        {
            foreach (var infoBinder in _enemyInfoBinders)
            {
                Destroy(infoBinder.gameObject);
            }
            _enemyInfoBinders.Clear();
            enemies.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 270);
        }
        

        
        private void OnEnable()
        {
            _btnInfo.onClick.AddListener(OnStartForced);
         //   _btnCloseInfo.onClick.AddListener(OnCloseInfo);
        }

        private void OnDisable()
        {
            _btnInfo.onClick.RemoveListener(OnStartForced);
       //     _btnCloseInfo.onClick.RemoveListener(OnCloseInfo);
        }
        
        private void OnShowInfo()
        {
            ViewModel.ShowPopupInfo();
        }
        private void OnStartForced()
        {
            if (!ViewModel.IsSelected.CurrentValue)
            {
                ViewModel.IsSelected.OnNext(true);
            } else
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

        public void FinishButtonAnimation()
        {
            _infoBlock.gameObject.SetActive(false);
            _btnImage.fillAmount = 1;
            
            //Пауза 0.5с
            
        }

        public void FinishInfoAnimation()
        {
            _infoPanel.gameObject.SetActive(false);
        }
    }
}