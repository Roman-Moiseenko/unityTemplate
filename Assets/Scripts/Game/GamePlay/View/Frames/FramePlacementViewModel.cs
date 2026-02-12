using System;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.Services;
using Game.GamePlay.View.Grounds;
using Game.GamePlay.View.Roads;
using Game.GamePlay.View.Towers;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames
{
    public class FramePlacementViewModel : IDisposable
    {
        private readonly PlacementService _placementService;
        public ReactiveProperty<Vector2Int> Position = new (Vector2Int.zero);
        public ReactiveProperty<bool> StartRemoveFlag = new(false);
        public ReactiveProperty<bool> FinishRemoveFlag = new(false);
        public Vector2Int StartPosition { get; set; }
        public ReactiveProperty<bool> Enable;

        public int TowerUniqueId;
       // public List<IMovingEntityViewModel> EntityViewModels = new();
        

        public ReactiveProperty<bool> IsSelected;

        public FramePlacementViewModel(TowerPlacementViewModel towerViewModel, FramePlacementService service)
        {
            //var t = towerViewModel.Placement;
            TowerUniqueId = towerViewModel.UniqueId;
            
            StartPosition = towerViewModel.Placement.Value;
            Position.Value = towerViewModel.Placement.CurrentValue;
            Enable = new ReactiveProperty<bool>(true);
            
            IsSelected = new ReactiveProperty<bool>(false);
            Position.Subscribe(position =>
            {
                var enabled = service.IsRoad(position);
                if (enabled) enabled = IsInPlacement(towerViewModel, position);
                Enable.Value = enabled;
                towerViewModel.EnabledPlacement.Value = enabled;
            });
     
        }
        
        private bool IsInPlacement(TowerViewModel towerViewModel, Vector2Int position)
        {
            if (towerViewModel.IsPlacement == false) return false;

            return Math.Abs(Position.CurrentValue.x - position.x) < 3 && 
                   Math.Abs(Position.CurrentValue.y - position.y) < 3;
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
        }
        

        public ReactiveProperty<bool> StartRemove()
        {
            FinishRemoveFlag.Value = false;
            StartRemoveFlag.OnNext(true);

            return FinishRemoveFlag;
        }

    }
}