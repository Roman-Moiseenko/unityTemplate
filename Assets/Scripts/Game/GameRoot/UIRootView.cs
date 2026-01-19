using System;
using TMPro;
using UnityEngine;

namespace Scripts.Game.GameRoot
{
    /**
     * Класс для прехаба UIRoot
     * содержит загрузочный UI для перехода между сценами
     * И контейнер для UI сцен
     */
    public class UIRootView :MonoBehaviour
    {
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private GameObject _loadingFirstScreen;
        [SerializeField] private Transform _uiSceneContainer;
        [SerializeField] private RectTransform _CanvasScaler;
        [SerializeField] private TMP_Text firstText;
            
        private void Awake()
        {
            HideLoadingScreen();
        }

        public void ShowLoadingScreen()
        {
            _loadingScreen.SetActive(true); //Показать UI перехода
        }
        
        public void HideLoadingScreen()
        {
            _loadingScreen.SetActive(false);
        }

        public void AttachSceneUI(GameObject sceneUI)
        {
            ClearSceneUI();
            sceneUI.transform.SetParent(_uiSceneContainer, false);
        }

        private void ClearSceneUI()
        {
            var childCount = _uiSceneContainer.childCount;
            for (var i = 0; i < childCount; i++)
            {
                Destroy(_uiSceneContainer.GetChild(i).gameObject);
            }
        }

        public void ShowLoadingFirstScreen()
        {
            _loadingFirstScreen.SetActive(true); //Показать UI перехода
        }

        public void HideLoadingFirstScreen()
        {
            _loadingFirstScreen.SetActive(false);
        }

        public void TextLoadingFirst(string text)
        {
            firstText.text = text;
        }

        public Vector3 GetScale()
        {
            return _CanvasScaler.localScale;
        }
    }
}