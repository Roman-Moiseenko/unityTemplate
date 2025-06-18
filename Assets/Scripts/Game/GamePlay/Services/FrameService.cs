using Game.GamePlay.View.Frames;
using Game.GamePlay.View.Towers;
using MVVM.CMD;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class FrameService
    {
        private readonly ICommandProcessor _cmd;

        public ReactiveProperty<TowerViewModel> TowerViewModel;
        public IObservableCollection<FrameViewModel> AllFrames => _allFrames;
        private readonly ObservableList<FrameViewModel> _allFrames = new();

        //public 

        public FrameService(ICommandProcessor cmd)
        {
            _cmd = cmd;
        }

        public FrameViewModel CreateFrame(Vector2Int position, int entityId)
        {
            var frame = new FrameViewModel(position, entityId);
            _allFrames.Add(frame);
            return frame;
        }
    }
}