using System;
using DG.Tweening;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.AttackAreas
{
    public class AttackAreaBinder : MonoBehaviour
    {
        [SerializeField] private Transform _area;
        [SerializeField] private Transform _areaDisabled;
        [SerializeField] private Transform _areaExpansion;
        private AttackAreaViewModel _viewModel;
        //private Animator _animator;
        private Vector3 _targetPosition;
        //private bool _isMoving = false;
        private bool _isHiding = false;
        private const int speed = 20;
        private const float smoothTime = 0.3f;
        private Vector3 _velocity;
        private IDisposable _disposable;
        
        public void Bind(AttackAreaViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            transform.position = new Vector3(viewModel.Position.CurrentValue.x, 0, viewModel.Position.CurrentValue.y);
            _viewModel.Position.Subscribe(newPosition =>
            {
                if (_viewModel.Moving)
                {
                    _targetPosition = new Vector3(newPosition.x, 0, newPosition.y);
                    transform.DOMove(_targetPosition, smoothTime).SetEase(Ease.OutQuad).SetUpdate(true);
                }
                else
                {
                    transform.position = new Vector3(newPosition.x, 0, newPosition.y);
                }
            }).AddTo(ref d);
            
            //Размер поверхности =  GetDimensions(_viewModel.RadiusExpansion.Value + _viewModel.RadiusArea.Value)
            //В шейдер передаем %% e = RadiusExpansion / RadiusArea и d =  RadiusDisabled / RadiusArea

            var material = _area.GetComponent<Renderer>().material;
            _viewModel.Radius.Subscribe(r =>
            {
                var radiusVector = GetDimensions(r.x + r.x * r.z);
                _area.transform.localScale = radiusVector;
                var _d = GetDimensions(r.y).x;
                var _e = GetDimensions(r.z).x;
  
                material.SetFloat("_Disabled", _d / radiusVector.x);
                material.SetFloat("_Expansion", _e / radiusVector.x);
                material.SetFloat("_Thickness", 0.04f / radiusVector.x); //Ободок
            }).AddTo(ref d);

            _viewModel.StartAnimationHide.Where(x => x).Subscribe(_ =>
            {
                _isHiding = true;
            }).AddTo(ref d);
            _disposable = d.Build();
        }
        
        private void Update()
        {
            if (_isHiding)
            {
                _area.transform.localScale = Vector3.Lerp(_area.transform.localScale, Vector3.zero, 0.20f);
                if (_area.transform.localScale.x < 0.3)
                {
                    _isHiding = false;
                    FinishAnimationHide();
                }
            }
        }
        
        private Vector3 GetDimensions(float radius)
        {
            var r = radius == 0 ? 0 : 1f + 2 * radius;
            return new Vector3(r, r, 1);
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }

        public void FinishAnimationHide()
        {
            _viewModel.Radius.Value = Vector3.zero;
            _viewModel.StartAnimationHide.Value = false;
        }
    }
}