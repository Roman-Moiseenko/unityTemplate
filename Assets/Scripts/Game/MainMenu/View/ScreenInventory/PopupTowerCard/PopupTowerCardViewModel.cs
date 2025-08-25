using Game.MainMenu.View.ScreenInventory.TowerCards;
using Game.State.Inventory;
using MVVM.UI;

namespace Game.MainMenu.View.ScreenInventory.PopupTowerCard
{
    public class PopupTowerCardViewModel : WindowViewModel
    {
        public readonly TowerCardViewModel CardViewModel;
        public override string Id => "PopupTowerCard";
        public override string Path => "MainMenu/ScreenInventory/Popups/";

        public PopupTowerCardViewModel(TowerCardViewModel viewModel)
        {
            CardViewModel = viewModel;
            

            //Заполняем данными из viewModel 
        }
    }
}