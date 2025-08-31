using Game.State.Inventory;
using R3;
using UnityEngine;

namespace Game.MainMenu.View.ScreenInventory.PopupBlacksmith.PrefabBinders
{
    public class TowerCardUpgradingViewModel
    {
        private TowerCardResourceViewModel _viewModel;
        public Vector3 PositionResource = Vector3.zero;  

        public ReactiveProperty<bool> IsSetCard = new(false);
        public ReactiveProperty<bool> IsNecessary = new(false); 
        public string ConfigId;
        public int Level = 1;
        public TypeEpicCard EpicLevel;
        public int Position = 0;
        public int TowerEntityId;
        public string NameCard { get; set; }

        public TowerCardUpgradingViewModel()
        {
            
        }

        

        public void SetTowerCardNecessary(TowerCardResourceViewModel viewModel)
        {
            ConfigId = viewModel.ConfigId;
            Level = 1;
            EpicLevel = viewModel.EpicLevel;
            IsNecessary.Value = true;
        }
        
        public void SetTowerCardViewModel(TowerCardResourceViewModel viewModel)
        {
            _viewModel = viewModel;
            NameCard = viewModel.NameCard;
            PositionResource = _viewModel.Position;
            Level = viewModel.Level;
            TowerEntityId = viewModel.TowerEntityId;
            IsSetCard.Value = true;
        }

        public void ResetViewModel()
        {
            _viewModel = null;
            PositionResource = Vector3.zero;
            Level = 1;
            TowerEntityId = 0;
            IsSetCard.Value = false;
        }

        public void ResetNecessary()
        {
            ResetViewModel();
            IsSetCard.Value = false;
            ConfigId = "";
            Level = 1;
            IsNecessary.Value = false;
        }

        public void ResetTowerCard()
        {
            _viewModel?.IsSelected.OnNext(false);
        }
    }
}