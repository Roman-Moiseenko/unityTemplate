using DI;
using Game.Settings.Gameplay.Maps;
using Game.State.GameStates;
using R3;

namespace Game.MainMenu.View.ScreenPlay.MapsGo
{
    public class MapCardViewModel
    {
        public int MapId;
        public string Title;
        public bool Finished = false;
        public int LastWave = 0;
        public bool Enabled;
        private readonly PlayUIManager _uiManager;

        public MapCardViewModel(MapSettings settingsMap, DIContainer container, MapState mapState)
        {
            Enabled = false;
            //Получаем данные из настроек для отображения
            MapId = settingsMap.MapId;
            Title = $"{settingsMap.MapId}. {settingsMap.InitialStateSettings.TitleLid}";
            
            //Загружаем данные игрока
            if (mapState != null)
            {
                Finished = mapState.Finished.CurrentValue;
                LastWave = mapState.LastWave;
                //TODO Стата о пройденных эатапах 
            }
            
            _uiManager = container.Resolve<PlayUIManager>();
        }
        
        public void ResumeInfoMapPopup()
        {
            //TODO Стата о пройденных эатапах 
            _uiManager.OpenPopupInfoMap(MapId);
        }
    }
}