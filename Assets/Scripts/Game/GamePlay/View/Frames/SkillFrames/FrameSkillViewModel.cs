using System;
using Game.GamePlay.Services;
using Game.Settings.Gameplay.Entities.Skill;
using Game.State.Maps.Skills;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames.SkillFrames
{
    
    public class FrameSkillViewModel : IDisposable
    {
        public ReactiveProperty<Vector2Int> Position { get; set; }
        public ReactiveProperty<bool> IsEnable;
        public string ConfigId;
        private DisposableBag _disposables;
        public bool OnRoad = false;  // public bool IsDirection = false; тоже самое
        public int MultiCells = 0;
        private SkillSettings _skillSettings;

        public ReadOnlyReactiveProperty<bool> IsPlacement;
        public ReadOnlyReactiveProperty<Vector2Int> Direction;
        
        
        public FrameSkillViewModel(string configId, SkillSettings skillSettings, FrameSkillService service)
        {
            ConfigId = configId;
            Position = new ReactiveProperty<Vector2Int>(Vector2Int.zero);
            IsEnable = new ReactiveProperty<bool>(false);
            _skillSettings = skillSettings;
            OnRoad = skillSettings.OnRoad;
            var cells = skillSettings.BaseParameters.Find(p => p.ParameterType == SkillParameterType.Cells);
            if (cells != null)
            {
                MultiCells = (int)(cells.Value - 1) / 2;
            }
            
            IsPlacement = service.IsPlacement;

            Direction = service.Direction;



        }
        
        public void MoveFrame(Vector2Int position)
        {
            Position.Value = position;
        }

        public void Dispose()
        {
            IsEnable?.Dispose();
            Position?.Dispose();
            _disposables.Dispose();
        }
    }
    

}