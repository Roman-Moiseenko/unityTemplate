using DI;
using Game.Common;
using MVVM.UI;
using UnityEngine;

namespace Game.GamePlay.View.UI.PopupSettings
{
    public class PopupSettingsViewModel : WindowViewModel
    {
        public override string Id => "PopupSettings";
        public override string Path => "Gameplay/Popups/";

        public PopupSettingsViewModel(DIContainer container) : base(container)
        {
            
        }

        public void RequestCommunity()
        {
            //TODO Аналитика учет
            Application.OpenURL(AppConstants.URL_COMMUNITY);
        }

        public void RequestSupport()
        {
            //TODO Аналитика учет
            Application.OpenURL(AppConstants.URL_SUPPORT);
        }
    }
}