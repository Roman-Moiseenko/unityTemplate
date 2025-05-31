using MVVM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.MainScreen
{
    public class MainScreenBinder : WindowBinder<MainScreenViewModel>
    {
        //Кнопки боттом-меню
        [SerializeField] private Button _btnShop;
        [SerializeField] private Button _btnInventory;
        [SerializeField] private Button _btnPlay;
        [SerializeField] private Button _btnClan;
        [SerializeField] private Button _btnResearch;
        
        //Кнопки топ-меню

        [SerializeField] private Button _btnAccount;
        
        private void OnEnable()
        {
            _btnShop.onClick.AddListener(OnShopButtonClicked);
            _btnInventory.onClick.AddListener(OnInventoryButtonClicked);
            _btnPlay.onClick.AddListener(OnPlayButtonClicked);
            _btnClan.onClick.AddListener(OnClanButtonClicked);
            _btnResearch.onClick.AddListener(OnResearchButtonClicked);
        }

        private void OnDisable()
        {
            _btnShop.onClick.RemoveListener(OnShopButtonClicked);
            _btnInventory.onClick.RemoveListener(OnInventoryButtonClicked);
            _btnPlay.onClick.RemoveListener(OnPlayButtonClicked);
            _btnClan.onClick.RemoveListener(OnClanButtonClicked);
            _btnResearch.onClick.RemoveListener(OnResearchButtonClicked);
        }
        
        private void OnResearchButtonClicked()
        {
            ViewModel.OpenResearch();
        }

        private void OnClanButtonClicked()
        {
            ViewModel.OpenClan();
        }

        private void OnPlayButtonClicked()
        {
            ViewModel.OpenPlay();
        }

        private void OnInventoryButtonClicked()
        {
            ViewModel.OpenInventory();
        }

        private void OnShopButtonClicked()
        {
            ViewModel.OpenShop();
        }


    }
}