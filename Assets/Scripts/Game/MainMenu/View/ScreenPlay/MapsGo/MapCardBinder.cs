using Game.Settings.Gameplay.Maps;
using MVVM.Storage;
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

        public void Bind(MapCardViewModel viewModel, Subject<int> startLevelGame, StorageManager storageManager)
        {
            _viewModel = viewModel;
            txtTitle.text = viewModel.Title;
            finished.gameObject.SetActive(viewModel.Finished);
            btnPlay.gameObject.SetActive(viewModel.Enabled);
            imageDisabled.gameObject.SetActive(!viewModel.Enabled);
            _startLevelGame = startLevelGame;
            var texture = storageManager.GetTextureFromCache(viewModel.UrlImage);
//            Debug.Log(texture);
            /*imageMap.sprite = Sprite.Create(storageManager.GetTextureFromCache(viewModel.UrlImage),
                imageMap.GetComponent<RectTransform>().rect, new Vector2(1,1)); */
            imageMap.sprite = Sprite.Create(texture,
                new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));

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
            btnInfoMap.onClick.AddListener(OnInfoMapPopup);
            //    _btnResumeGame.onClick.AddListener(OnResumeGameButtonClicked);
        }

        private void OnDisable()
        {
            btnPlay.onClick.RemoveListener(OnResumeBattleClicked);
            btnInfoMap.onClick.RemoveListener(OnInfoMapPopup);
            //    _btnResumeGame.onClick.RemoveListener(OnResumeGameButtonClicked);
        }

        private void OnInfoMapPopup()
        {
            _viewModel.ResumeInfoMapPopup();
        }

        private void OnResumeBattleClicked()
        {
            _startLevelGame.OnNext(_viewModel.MapId);
        }

        //TODO Кнопки по нажатию наведению (ИНФО о ВОЛНЕ) Запуск Волны

    }
}