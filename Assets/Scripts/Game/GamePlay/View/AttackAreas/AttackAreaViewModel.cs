using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.GamePlay.View.AttackAreas
{
    public class AttackAreaViewModel
    {

        public ReactiveProperty<float> RadiusArea;
        public ReactiveProperty<float> RadiusDisabled;
        public ReactiveProperty<float> RadiusExpansion;
        public ReactiveProperty<Vector2Int> Position;
        private Vector3 _memoryRadius;
        public bool Moving;
        public AttackAreaViewModel(Vector2Int position, float radiusArea = 0, float radiusDisabled = 0, float radiusExpansion = 0)
        {
            Position = new ReactiveProperty<Vector2Int>(position);
            RadiusArea = new ReactiveProperty<float>(radiusArea);
            RadiusDisabled = new ReactiveProperty<float>(radiusDisabled);
            RadiusExpansion = new ReactiveProperty<float>(radiusExpansion);
        }

        public void Hide()
        {
            RadiusArea.Value = 0;
            RadiusDisabled.Value = 0;
            RadiusExpansion.Value = 0;
        }

        public void Restore()
        {
            RadiusArea.Value = _memoryRadius.x;
            RadiusDisabled.Value = _memoryRadius.y;
            RadiusExpansion.Value = _memoryRadius.y;
        }

        public void SetRadius(Vector3 radius)
        {
            _memoryRadius = radius;
            RadiusArea.Value = radius.x;
            RadiusDisabled.Value = radius.y;
            RadiusExpansion.Value = radius.z;
        }

        public void SetStartPosition(Vector2Int positionValue)
        {
            Moving = false;
            Position.Value = positionValue;
        }

        public void SetPosition(Vector2Int positionValue)
        {
            Moving = true;
            Position.Value = positionValue;
        }
    }
}