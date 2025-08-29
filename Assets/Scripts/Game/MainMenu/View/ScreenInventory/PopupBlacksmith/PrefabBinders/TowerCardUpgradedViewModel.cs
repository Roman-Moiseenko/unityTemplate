using System;
using Game.State.Inventory;
using R3;

namespace Game.MainMenu.View.ScreenInventory.PopupBlacksmith.PrefabBinders
{
    public class TowerCardUpgradedViewModel
    {
        private TowerCardResourceViewModel _viewModel;

        public ReactiveProperty<bool> IsActive = new(false);
        public string ConfigId;
        public int Level;
        public TypeEpicCard EpicLevel;

        public TowerCardUpgradedViewModel()
        {
            
        }

        public void SetTowerCardActive(TowerCardResourceViewModel viewModel)
        {
            ConfigId = viewModel.ConfigId;
            Level = 1;
            EpicLevel = viewModel.EpicLevel; //TODO Увеличчить на 1 шт
            IsActive.Value = true;
        }

        

        public void ResetActive()
        {

            Level = 1;
            IsActive.Value = false;
        }

        
        
    }
}