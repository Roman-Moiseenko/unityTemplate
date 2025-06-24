using Game.GamePlay.View.Towers;
using R3;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.GamePlay.View.Frames
{
    public class FrameBlockTower : FrameBlock
    {
        public TowerViewModel TowerViewModel { get; }
        public FrameViewModel FrameViewModel { get; }
        
        public FrameBlockTower(Vector2Int position, TowerViewModel towerViewModel, FrameViewModel frameViewModel) : base(position)
        {
            TowerViewModel = towerViewModel;
            FrameViewModel = frameViewModel;
            IsRotate = false;
            Enable.Subscribe(newValue => FrameViewModel.Enable.Value = newValue);
        }

        public void OnDestroy()
        {
            
        }
        
        public override bool FrameIs(FrameType frameType)
        {
            return FrameType.Tower == frameType;
        }

        public override void Move(Vector2Int position)
        {
            TowerViewModel.Position.Value = position;
            FrameViewModel.Position.Value = position;
        }

        public override void Selected(bool value = true)
        {
            FrameViewModel.Selected(value);
        }
        
        public override bool IsPosition(Vector2Int position) 
        {
            return FrameViewModel.Position.Value == position;
        }
    }
}