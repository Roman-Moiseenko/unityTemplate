using DI;
using Game.Settings;
using MVVM.UI;

namespace Game.MainMenu.View.ScreenPlay.PopupInfoMap
{
    public class PopupInfoMapViewModel : WindowViewModel
    {
        public override string Id => "PopupInfoMap";
        public override string Path => "MainMenu/ScreenPlay/Popups/";

        public string TitleMap;
        
        public PopupInfoMapViewModel(int mapId, DIContainer container)
        {
            var mapsSettings = container.Resolve<ISettingsProvider>().GameSettings.MapsSettings;
            var map = mapsSettings.Maps.Find(v => v.MapId == mapId);
            TitleMap = $"{mapId}. {map.InitialStateSettings.TitleLid}";

        }
    }
}