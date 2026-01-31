using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.GamePlay.View.Grounds;
using Game.GamePlay.View.Roads;
using Game.GamePlay.View.Towers;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames
{
    public class FramePlacementBinder : MonoBehaviour
    {
        [SerializeField] private GameObject frame;
        
        private Vector3 _targetPosition;
        private bool _isMoving;
        private const int Speed = 20;
        private const float SmoothTime = 0.3f;
        private Vector3 _velocity;
        private IDisposable _disposable;
        private bool _showCloudDust;
        
        private AreaBinder _areaBinder;

        public void Bind(FramePlacementViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            var material = frame.GetComponent<Renderer>().material;
            
            Observable.Merge(viewModel.Enable, viewModel.IsSelected).Subscribe(v =>
            {
                material.SetInt("_Enabled", viewModel.Enable.CurrentValue ? 1 : 0);
                material.SetInt("_Selected", viewModel.IsSelected.CurrentValue ? 1 : 0);
                
            }).AddTo(ref d);
            viewModel.Position.Subscribe(newPosition =>
            {
                _targetPosition = new Vector3(newPosition.x, transform.position.y, newPosition.y);
                transform.DOMove(_targetPosition, SmoothTime).SetEase(Ease.OutQuad).SetUpdate(true);
            }).AddTo(ref d);
            
            
            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x,
                transform.position.y,
                viewModel.Position.CurrentValue.y
            );
            
            _disposable = d.Build();
        }
        
        private void Update()
        {
            if (_isMoving)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, SmoothTime, Speed, Time.unscaledTime);
                if (_velocity.magnitude < 0.0005)
                {
                    _isMoving = false;
                    transform.position = _targetPosition;
                }
            }
        }
        
        private void OnDestroy()
        {
            _disposable.Dispose();

        }
    }
}