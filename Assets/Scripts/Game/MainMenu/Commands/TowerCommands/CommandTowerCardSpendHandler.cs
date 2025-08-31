using Game.State.Root;
using MVVM.CMD;

namespace Game.MainMenu.Commands.TowerCommands
{
    public class CommandTowerCardSpendHandler : ICommandHandler<CommandTowerCardSpend>
    {
        private readonly GameStateProxy _gameState;

        public CommandTowerCardSpendHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }
        
        public bool Handle(CommandTowerCardSpend command)
        {
            //При удалении карты, удаляем ее из всех колод
            foreach (var deckPair in _gameState.Inventory.DeckCards)
            {
                var deck = deckPair.Value;
                deck.ExtractFromDeck(command.UniqueId);
            }
            
            _gameState.Inventory.RemoveItem(command.UniqueId);
            return true;
        }
    }
}