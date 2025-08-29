using MVVM.CMD;

namespace Game.MainMenu.Commands.InventoryCommands
{
    public class CommandChangeCardsInDeck : ICommand
    {
        public int TowerCardId;

        public CommandChangeCardsInDeck(int towerCardId)
        {
            TowerCardId = towerCardId;
        }
    }
}