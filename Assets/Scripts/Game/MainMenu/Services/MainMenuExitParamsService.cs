using System;
using System.Linq;
using DI;
using Game.GamePlay.Root;
using Game.MainMenu.Root;
using Game.State;
using Game.State.Inventory.TowerCards;
using Game.State.Root;

namespace Game.MainMenu.Services
{
    public class MainMenuExitParamsService
    {
        private readonly DIContainer _container;
        private readonly GameStateProxy _gameState;

        public MainMenuExitParamsService(DIContainer container)
        {
            _container = container;
            _gameState = container.Resolve<IGameStateProvider>().GameState;
        }

        public MainMenuExitParams GetExitParams(int currentIdMap)
        {
            var gameplayEnterParams = new GameplayEnterParams(currentIdMap);
            var gameState = _container.Resolve<IGameStateProvider>().GameState;
            var deckCard = gameState.DeckCards[gameState.BattleDeck.CurrentValue];
            
            //Переносим башни
            foreach (var keyValue in deckCard.TowerCardIds)
            {
                var towerUniqueId = keyValue.Value;
                var towerCard = gameState.InventoryItems.FirstOrDefault(item => item.UniqueId == towerUniqueId);
                if (towerCard == null) throw new Exception($"Отсутствует в инвентаре башня с id = {towerUniqueId}");
                gameplayEnterParams.Towers.Add((TowerCardData)towerCard?.Origin);
            }
            //TODO Переносим навыки
            
            //TODO Переносим героя
            
            //TODO Передаем сохраненные настройки геймплея 
            gameplayEnterParams.GameSpeed = _gameState.GameSpeed.CurrentValue;
            
            
            var mainMenuExitParams = new MainMenuExitParams(gameplayEnterParams);

            return mainMenuExitParams;
        }
    }
}