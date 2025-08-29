using Game.Settings;
using Game.State.Root;
using MVVM.CMD;

namespace Game.MainMenu.Commands.InventoryCommands
{
    public class CommandChangeCardsInDeckHandler : ICommandHandler<CommandChangeCardsInDeck>
    {
        
        private readonly GameStateProxy _gameState;
        private readonly GameSettings _gameSettings;

        public CommandChangeCardsInDeckHandler(
            GameStateProxy gameState, 
            GameSettings gameSettings
        )
        {
            _gameState = gameState;
            _gameSettings = gameSettings;
        }
        public bool Handle(CommandChangeCardsInDeck command)
        {
            /*var towerView = _gameState.Inventory.Items
                .FirstOrDefault(t => t.IdTowerCard == uniqueId)!; */

         /*   if (_currentDeck.TowerCardInDeck(uniqueId))
            {
                _currentDeck.ExtractFromDeck(uniqueId);
                towerView.NumberCardDeck = 0;
                towerView.IsDeck.OnNext(false);
            }
            else
            {
                towerView.NumberCardDeck = _currentDeck.PushToDeck(uniqueId);
                towerView.IsDeck.OnNext(true);
            } */
         return true;
        }
    }
}