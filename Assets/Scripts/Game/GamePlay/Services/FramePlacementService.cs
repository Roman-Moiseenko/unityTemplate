using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.TowerStates;
using Game.GamePlay.View.Frames;
using Game.GamePlay.View.Towers;
using Game.State.Root;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class FramePlacementService
    {
        public IObservableCollection<FramePlacementViewModel> ViewModels => _viewModels;  
        private readonly ObservableList<FramePlacementViewModel> _viewModels = new();
        private readonly PlacementService _placementService;
        private FramePlacementViewModel _viewModel;

        public FramePlacementService(            
            GameplayStateProxy gameplayState,
            PlacementService placementService,
            FsmTower fsmTower,
            TowersService towersService
            )
        {
            _placementService = placementService;
            fsmTower.Fsm.StateCurrent.Subscribe(newState =>
            {
                if (newState.GetType() == typeof(FsmTowerPlacement))
                {
                    CreatePlacement((TowerPlacementViewModel)fsmTower.GetTowerViewModel());
                }
                else
                { 
                    if (_viewModel == null) return;
                    if (newState.GetType() == typeof(FsmTowerPlacementEnd))
                    { 
                        //Сохраняем новое значение
                        towersService.SetPlacement(_viewModel.TowerUniqueId, _viewModel.Position.CurrentValue);
                    }
                    RemovePlacement();
                }
            });
        }


        public void MoveFrame(Vector2Int position)
        {
            _viewModel.MoveFrame(position);
        }
        
        public bool TrySelectedFrame(Vector2Int position)
        {
            if (_viewModel == null) return false;
            if (_viewModel.Position.CurrentValue == position)
            {
                _viewModel.Selected(true);
                return true;
            }

            return false;
        }

        public bool TryUnSelectedFrame()
        {
            if (_viewModel == null) return false;
            if (_viewModel.IsSelected.CurrentValue)
            {
                _viewModel?.Selected(false);
                return true;
            }

            return false;

        }

        public void CreatePlacement(TowerPlacementViewModel towerViewModel)
        {
            _viewModel = new FramePlacementViewModel(towerViewModel , this);
            _viewModels.Add(_viewModel);
            
        }

        public bool IsSelected()
        {
            if (_viewModel == null) return false;
            return _viewModel.IsSelected.CurrentValue;
        }

        private void RemovePlacement()
        {
            _viewModels.Remove(_viewModel);
            _viewModel = null;
        }

        public bool IsRoad(Vector2Int position)
        {
            return _placementService.IsRoad(position);
        }
    }
}