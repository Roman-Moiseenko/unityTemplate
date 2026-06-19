using System;
using DG.Tweening;
using Game.GamePlay.View.Towers;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames.HeroFrames
{
    public class FrameHeroBinder : MonoBehaviour
    {
        [SerializeField] private GameObject frame;
        
        private Vector3 _targetPosition;
        private const int Speed = 20;
        private const float SmoothTime = 0.3f;
        private Vector3 _velocity;
        private IDisposable _disposable;
        private bool _showCloudDust;
        
        private AreaBinder _areaBinder;

        public void Bind(FrameHeroViewModel viewModel)
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
        
        private void OnDestroy()
        {
            _disposable.Dispose();

        }
    }
}