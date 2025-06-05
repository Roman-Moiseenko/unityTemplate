using Game.State.Maps.Roads;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Roads
{
    public class RoadViewModel
    {
        private readonly RoadEntity _roadEntity;
        public ReadOnlyReactiveProperty<Vector2Int> Position { get; }

        public RoadViewModel(
            RoadEntity roadEntity
            )
        {
            //TODO
            _roadEntity = roadEntity;
            
            Position = roadEntity.Position;
        }
    }
}