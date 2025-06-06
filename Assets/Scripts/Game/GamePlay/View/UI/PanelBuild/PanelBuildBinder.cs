using MVVM.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelBuild
{
    public class PanelBuildBinder : PanelBinder<PanelBuildViewModel>
    {
        [SerializeField] private Button _btnBuild1;
        [SerializeField] private Button _btnBuild2;
        [SerializeField] private Button _btnBuild3;
   
        public override void Show()
        {
            if (isShow) return;
            //Получаем у ViewModel данные для отображения на карточках, грузим картинки
            base.Show();
            panel.pivot = new Vector2(0.5f, 0);
        }
        
        public override void Hide()
        {
            if (!isShow) return;
            base.Hide();
            panel.pivot = new Vector2(0.5f, 1);
        }


        private void OnEnable()
        {
            _btnBuild1.onClick.AddListener(OnClickButtonBuild1);            
            _btnBuild2.onClick.AddListener(OnClickButtonBuild2);            
            _btnBuild3.onClick.AddListener(OnClickButtonBuild3);            
        }


        private void OnDisable()
        {
            _btnBuild1.onClick.RemoveListener(OnClickButtonBuild1);
            _btnBuild2.onClick.RemoveListener(OnClickButtonBuild2);
            _btnBuild3.onClick.RemoveListener(OnClickButtonBuild3);
        }
        
        private void OnClickButtonBuild3()
        {
            ViewModel.OnBuild1();
        }

        private void OnClickButtonBuild2()
        {
            ViewModel.OnBuild2();
        }

        private void OnClickButtonBuild1()
        {
            ViewModel.OnBuild3();
        }
    }
}