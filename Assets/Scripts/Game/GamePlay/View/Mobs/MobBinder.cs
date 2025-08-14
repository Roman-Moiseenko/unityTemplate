using System;
using Game.State.Maps.Mobs;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.GamePlay.View.Mobs
{
    public class MobBinder : MonoBehaviour
    {
        [SerializeField] private Transform _healthBar;
        MaterialPropertyBlock matBlock;
        MeshRenderer meshRenderer;
		public MobViewModel _viewModel;
        private Vector3 _targetPosition;
        private HealthBar _healthBarBinder;

        private Quaternion _targetDirection;

        private float _mobY;
        private int _currentIndexListPoint;
        
        IDisposable disposable;

        public void Bind(MobViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            _mobY = _viewModel.IsFly ? 0.9f : 0.1f;
            transform.position = new Vector3(viewModel.StartPosition.x, _mobY, viewModel.StartPosition.y);

            _healthBarBinder = _healthBar.GetComponent<HealthBar>();
            _healthBarBinder.Bind(
                _viewModel.CameraService.Camera, 
                _viewModel.MaxHealth, 
                _viewModel.CurrentHealth,
                _viewModel.Level
                );

            
            //поворачиваем модель
            transform.rotation = Quaternion.LookRotation(new Vector3(viewModel.Direction.CurrentValue.x, 0, viewModel.Direction.CurrentValue.y));
            
            //Вращаем в движении
            viewModel.Direction.Skip(1).Subscribe(newValue =>
            {
                var direction = new Vector3(newValue.x, 0, newValue.y);
                _targetDirection = Quaternion.LookRotation(direction);
            }).AddTo(ref d);
            
            viewModel.Position.Subscribe(newValue =>
            {
                transform.position = new Vector3(newValue.x, _mobY, newValue.y);
            }).AddTo(ref d);

            viewModel.AnimationDelete.Subscribe(v =>
            {
                if (v)
                {
                    //TODO Анимация удаления объекта После окончания:
                   // Debug.Log("Моб " + viewModel.MobEntityId + " Уничтожается");
                   viewModel.FinishCurrentAnimation.Value = true;
                }
            }).AddTo(ref d);

            viewModel.State.Subscribe(newState =>
            {
                //TODO Переключаем анимацию от состояния моба.
                if (newState == MobState.Attacking)
                {
                 //   Debug.Log("Моб " + viewModel.MobEntityId + " Аттакует");
                }
            }).AddTo(ref d);
            disposable = d.Build();
        }

        public void Update()
        {
            _healthBarBinder.OnUpdate();
        }

        private void OnDestroy()
        {
           disposable.Dispose();
        }
        
        
        /*
        private Vector3 GetTargetPosition()
        {
            var newValue = _viewModel.RoadPoints[_currentIndexListPoint].Point;
            _targetPosition = new Vector3(newValue.x, _mobY, newValue.y);
            return _targetPosition;
        }
        

        private void AlignCamera() {
            if (_viewModel.CameraService.Camera != null) {
                var camXform = _viewModel.CameraService.Camera.transform;
                var forward = _healthBar.transform.position - camXform.position;
                forward.Normalize();
                var up = Vector3.Cross(forward, camXform.right);
                _healthBar.transform.rotation = Quaternion.LookRotation(forward, up);
            }
        }
        

        private void UpdateParams() {
            meshRenderer.GetPropertyBlock(matBlock);
            matBlock.SetFloat("_Fill", _viewModel.CurrentHealth.CurrentValue / _viewModel.MaxHealth);
            meshRenderer.SetPropertyBlock(matBlock);
        }
        
        */
    }
}