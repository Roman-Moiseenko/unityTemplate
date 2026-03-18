using MVVM.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PopupPause
{
    public class PopupPauseBinder : PopupBinder<PopupPauseViewModal>
    {
        [SerializeField] private Button btnToExit;
        [SerializeField] private Button btnToSettings;
        [SerializeField] private Button btnToStatistic;
        [SerializeField] private TMP_Text txtCaption;
        
        [SerializeField] private TMP_Text countKills;
        [SerializeField] private TMP_Text countResurrection;
        [SerializeField] private TMP_Text countTowers;
        [SerializeField] private TMP_Text countRoads;

        
        
        [SerializeField] private Button _btnWin;

        [SerializeField] private Button _btnLose;
        //Поля кнопок во всплывающем окне
        //Например при проигрыше - а) выйти, б) посмотреть рекламу и продолжить, в) купить продолжение за кристаллы
        //Переписываем OnBind(), когда надо реализовать свое

        protected override void OnBind(PopupPauseViewModal viewModal)
        {
            base.OnBind(viewModal);
            var resurrection = viewModal.GameplayState.Castle.CountResurrection.CurrentValue;
            var stat = viewModal.GameplayState.StatisticGame;
            txtCaption.text = $"Глава {viewModal.GameplayState.MapId.CurrentValue}";
            //TODO Получаем статистические данные
            countResurrection.text = $"{2 - resurrection}/2";
            countKills.text = stat.CountKills.CurrentValue.ToString();
            countTowers.text = stat.CountTowers.CurrentValue.ToString();
            countRoads.text = stat.CountRoads.CurrentValue.ToString();
        }
        

        //Подписываемся на нажатия кнопок и вызываем функции из View Модели
        private void OnEnable()
        {
            btnToExit.onClick.AddListener(OnToExitClick);
            btnToSettings.onClick.AddListener(OnToSettingsClick);
            btnToStatistic.onClick.AddListener(OnToStatisticClick);
            
            _btnWin.onClick.AddListener(OnWin);
            _btnLose.onClick.AddListener(OnLose);
        }

        private void OnDisable()
        {
            btnToExit.onClick.RemoveListener(OnToExitClick);
            btnToSettings.onClick.RemoveListener(OnToSettingsClick);
            btnToStatistic.onClick.RemoveListener(OnToStatisticClick);

            _btnWin.onClick.RemoveListener(OnWin);
            _btnLose.onClick.RemoveListener(OnLose);
        }

        private void OnToExitClick()
        {
            ViewModel.RequestClose();
            ViewModel.RequestToExit();
        }
        private void OnToSettingsClick()
        {
            ViewModel.RequestClose();
            ViewModel.RequestToSettings();
        }
        private void OnToStatisticClick()
        {
            ViewModel.RequestToStatistic();
        }
        
        private void OnWin()
        {
            ViewModel.RequestClose();
            ViewModel.Win();
        }

        private void OnLose()
        {
            ViewModel.RequestClose();
            ViewModel.Lose();
        }
        
        protected override void OnCloseButtonClick()
        {
            //
            ViewModel.RequestClose();
        }
    }
}