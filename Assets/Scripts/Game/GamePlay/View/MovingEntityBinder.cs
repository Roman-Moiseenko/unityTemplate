using UnityEngine;

namespace Game.GamePlay.View
{
    public abstract class MovingEntityBinder<T> : MonoBehaviour
    {
        private Vector3 _targetPosition;
      //  private <T> _viewModel;
        private bool _isMoving = false;
        
        private const int speed = 20;
        private const float smoothTime = 0.2f;
        private Vector3 _velocity;
    }
}