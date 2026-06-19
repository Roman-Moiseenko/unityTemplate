using System;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.HeroStates;
using Game.GamePlay.Fsm.TowerStates;
using Game.GamePlay.View.Frames;
using Game.GamePlay.View.Frames.HeroFrames;
using Game.GamePlay.View.Frames.TowerFrames;
using Game.GamePlay.View.Towers;
using Game.State.Gameplay;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class FrameHeroService : IDisposable
    {
        public readonly ReactiveProperty<FrameHeroViewModel> ViewModel = new(null);

        //public IObservableCollection<FramePlacementViewModel> ViewModels => _viewModels;  
        private readonly ObservableList<FramePlacementViewModel> _viewModels = new();
        private readonly PlacementService _placementService;
        private FrameHeroViewModel _viewModel;
        private DisposableBag _disposables = new();

        public FrameHeroService(
            GameplayStateProxy gameplayState,
            PlacementService placementService,
            FsmHero fsmHero,
            HeroesService heroesService
        )
        {
            _placementService = placementService;
            fsmHero.Fsm.StateCurrent.Subscribe(newState =>
            {
                if (newState.GetType() == typeof(FsmHeroPlacement))
                {
                    _viewModel = new FrameHeroViewModel(heroesService.HeroViewModel, this);
                    ViewModel.Value = _viewModel;
                    return;
                }

                if (_viewModel == null) return;
                
                if (newState.GetType() == typeof(FsmHeroPlacementEnd) && _viewModel != null)
                {
                    heroesService.SetPlacement(_viewModel.Position.CurrentValue);
                    RemoveViewModel();
                }

                if (newState.GetType() == typeof(FsmHeroUnSelected))
                {
                    RemoveViewModel();
                }
            }).AddTo(ref _disposables);
        }

        private void RemoveViewModel()
        {
            ViewModel.Value = null;
            _viewModel?.Dispose();
            _viewModel = null;
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


        public bool IsSelected()
        {
            if (_viewModel == null) return false;
            return _viewModel.IsSelected.CurrentValue;
        }


        public bool IsRoad(Vector2Int position)
        {
            var isRoad = _placementService.IsRoad(position);
            var isEdge = _placementService.IsEdge(position); 
            
            return isRoad && !isEdge;
        }

        public void Dispose()
        {
            _disposables.Dispose();
            foreach (var viewModel in _viewModels)
            {
                viewModel.Dispose();
            }

            _viewModels.Clear();
            _viewModel?.Dispose();
            _viewModel = null;
        }
    }
}