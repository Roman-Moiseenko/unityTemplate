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
        /**
         * Данные об игроке, его достижения за всю игру
         */
        public GameStateProxy GameState { get; } //Данные игрока

        public Observable<GameStateProxy> LoadGameState();
        public Observable<bool> SaveGameState();
        public Observable<bool> ResetGameState();


        /**
         * Персональные настройки игры, Settings
         */
        public GameSettingsStateProxy SettingsState { get; } //Настройки игры

        public Observable<GameSettingsStateProxy> LoadSettingsState();
        public Observable<bool> SaveSettingsState();
        public Observable<bool> ResetSettingsState();
        
        /**
         * Данные о сессии геймплея, если был выход из нее по причине прерывание сессии
         */
        public GameplayStateProxy GameplayState { get; } //Данные об сессии игры

        public Observable<GameplayStateProxy> LoadGameplayState();
        public Observable<bool> SaveGameplayState();
        public Observable<bool> ResetGameplayState();
    }
}