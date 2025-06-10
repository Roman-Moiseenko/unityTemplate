using DI;
using Game.State;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    /**
     * Сервис для подписок на события которые необходимо сохранить.
     */
    public static class GameplaySaveService
    {
        public static void Run(DIContainer container)
        {
            var provider = container.Resolve<IGameStateProvider>();
            provider.GameState.GameplayStateProxy.GameSpeed.Subscribe(newSpeed =>
            {
                if (newSpeed != 0)
                {
                    provider.SaveGameState();
                }
            });
            //TODO Добавить другие параметры при сохранении
            
            
        }
    }
}