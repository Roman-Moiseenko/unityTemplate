using System;
using Game.GamePlay.Services;
using Game.GamePlay.View.Hero;
using Game.GamePlay.View.Towers;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames.HeroFrames
{
    public class FrameHeroViewModel : IDisposable
    {
        private readonly PlacementService _placementService;
        public ReactiveProperty<Vector2Int> Position = new (Vector2Int.zero);
        public ReactiveProperty<bool> StartRemoveFlag = new(false);
        public ReactiveProperty<bool> FinishRemoveFlag = new(false);
        public Vector2Int StartPosition { get; set; }
        public ReactiveProperty<bool> Enable;

        public int TowerUniqueId;
       // public List<IMovingEntityViewModel> EntityViewModels = new();
       private HeroViewModel _heroViewModel;

        public ReactiveProperty<bool> IsSelected;

        private DisposableBag _disposables = new();

        public FrameHeroViewModel(HeroViewModel heroViewModel, FrameHeroService service)
        {
            _heroViewModel = heroViewModel;
            //var t = towerViewModel.Placement;
            

            StartPosition = heroViewModel.Placement.Value;
            Position.Value = heroViewModel.Placement.CurrentValue;
            Enable = new ReactiveProperty<bool>(true);

            IsSelected = new ReactiveProperty<bool>(false);
            Position.Subscribe(position =>
            {
                var enabled = service.IsRoad(position);
                
                Enable.Value = enabled;
                //heroViewModel.EnabledPlacement.Value = enabled;
            }).AddTo(ref _disposables);
        }
        

        public void MoveFrame(Vector2Int position)
        {
            Position.Value = position;
        }

        public void Selected(bool value)
        {
            IsSelected.Value = value;
        }

        public void Dispose()
        {
            Enable?.Dispose();
            IsSelected?.Dispose();
            Position?.Dispose();
            _disposables.Dispose();
        }


        public ReactiveProperty<bool> StartRemove()
        {
            FinishRemoveFlag.Value = false;
            StartRemoveFlag.OnNext(true);

            return FinishRemoveFlag;
        }

    }
}
