using MVVM.UI;
using TMPro;
using UnityEngine;

namespace Game.MainMenu.View.ScreenPlay.PopupInfoMap
{
    public class PopupInfoMapBinder : PopupBinder<PopupInfoMapViewModel>
    {
        [SerializeField] private TMP_Text titleMap;
        
        protected override void OnBind(PopupInfoMapViewModel viewModel)
        {
            titleMap.text = viewModel.TitleMap;
        }
    }
}