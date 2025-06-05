using DI;
using Game.State;
using R3;

namespace Game.GamePlay.Services
{
    /**
     * Сервис для подписок на события которые необходимо сохранить.
     */
    public static class GameplaySaveService
    {
        public static void Run(DIContainer container)
        {
            var _provider = container.Resolve<IGameStateProvider>();
            _provider.GameState.GameplayState.GameSpeed.Subscribe(newSpeed =>
            {
                if (newSpeed != 0) _provider.SaveGameState();
            });
            //TODO Добавить другие параметры при сохранении
            
            
        }
    }
}