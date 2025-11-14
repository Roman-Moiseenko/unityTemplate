using Game.Settings.Gameplay.Maps;
using Game.State.GameStates;

namespace Game.MainMenu.View.ScreenPlay.MapsGo
{
    public class MapCardViewModel
    {
        public int MapId;
        public string Title;
        public bool Finished = false;
        public int LastWave = 0;
        public bool Enabled;
        
        public MapCardViewModel(MapSettings settingsMap, bool enabled, MapState mapState = null)
        {
            //Получаем данные из настроек для отображения
            MapId = settingsMap.MapId;
            Title = $"{settingsMap.MapId}. {settingsMap.InitialStateSettings.TitleLid}";
            Enabled = enabled;
            //Загружаем данные игрока
            if (mapState != null)
            {
                Finished = mapState.Finished.CurrentValue;
                LastWave = mapState.LastWave;
                //TODO Стата о пройденных эатапах 
            }
            
            
            //
        }

    /*    public void OnResumeBattleClicked()
        {
            
        }*/
    }
}