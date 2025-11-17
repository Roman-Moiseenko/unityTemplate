using Game.Settings.Gameplay.Maps;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenPlay.MapsGo
{
    public class MapCardBinder : MonoBehaviour
    {
        [SerializeField] private Button btnPlay;
        [SerializeField] private TMP_Text txtTitle;
        [SerializeField] private Image imageMap;
        [SerializeField] private Image imageDisabled;
        [SerializeField] private Transform containerSymbols; //List<эмблемы волны>
        [SerializeField] private Transform finished;

        [SerializeField] private Button btnInfoMap;
        private MapCardViewModel _viewModel;
        private Subject<int> _startLevelGame;

        public void Bind(MapCardViewModel viewModel, Subject<int> startLevelGame)
        {
            _viewModel = viewModel;
            txtTitle.text = viewModel.Title;
            finished.gameObject.SetActive(viewModel.Finished);
            btnPlay.gameObject.SetActive(viewModel.Enabled);
            imageDisabled.gameObject.SetActive(!viewModel.Enabled);
            _startLevelGame = startLevelGame;
            //TODO ... Вывод всех данных
        }
        public void SetEnabled()
        {
            btnPlay.gameObject.SetActive(true);
            imageDisabled.gameObject.SetActive(false);
        }

        public void SetFinished()
        {
            finished.gameObject.SetActive(true);
        }
        private void OnEnable()
        {
            btnPlay.onClick.AddListener(OnResumeBattleClicked);
            //    _btnResumeGame.onClick.AddListener(OnResumeGameButtonClicked);
        }

        private void OnDisable()
        {
            btnPlay.onClick.RemoveListener(OnResumeBattleClicked);
            //    _btnResumeGame.onClick.RemoveListener(OnResumeGameButtonClicked);
        }

        private void OnResumeBattleClicked()
        {
            _startLevelGame.OnNext(_viewModel.MapId);
            //_viewModel.OnResumeBattleClicked();
        }

        //TODO Кнопки по нажатию наведению (ИНФО о ВОЛНЕ) Запуск Волны

    }
}