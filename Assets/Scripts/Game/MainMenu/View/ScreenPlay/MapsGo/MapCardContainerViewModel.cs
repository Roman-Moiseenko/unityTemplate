using DI;

namespace Game.MainMenu.View.ScreenPlay.MapsGo
{
    public class MapCardContainerViewModel
    {
        private readonly DIContainer _container;

        public MapCardContainerViewModel(DIContainer container)
        {
            _container = container;
            //Загружаем список карт из настроек
            //Создаем ViewModel карты и в список
            
            //Загружаем прогресс Игрока (текущую карту)
        }
    }
}