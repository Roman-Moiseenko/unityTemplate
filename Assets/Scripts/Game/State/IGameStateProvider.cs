using Game.State.Root;
using R3;

namespace Game.State
{
    /**
     * Интерфейс репозитория всех данных в игре
     * Сохранение, Загрузка и Сброс до начальных
     */
    public interface IGameStateProvider
    {
        public GameStateProxy GameState { get; } //Данные игрока
        public GameSettingsStateProxy SettingsState { get; } //Настройки игры
        public Observable<GameStateProxy> LoadGameState();
        public Observable<GameSettingsStateProxy> LoadSettingsState();
        public Observable<bool> SaveGameState();
        public Observable<bool> SaveSettingsState();

        public Observable<bool> ResetGameState();
        public Observable<bool> ResetSettingsState();
    }
}