using UnityEngine;

namespace Game.GamePlay.View
{
    public interface IMovingEntityViewModel
    {
        public void SetPosition(Vector2Int position);
        public Vector2Int GetPosition();
    }
}