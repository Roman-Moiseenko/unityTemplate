﻿using System;
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
        private int _currentIndexListPoint = 0;

        public void Bind(MobViewModel viewModel)
        {
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
            _viewModel.Direction.Skip(1).Subscribe(newValue =>
            {
                var direction = new Vector3(newValue.x, 0, newValue.y);
                _targetDirection = Quaternion.LookRotation(direction);
                
            });
            
            viewModel.Position.Subscribe(newValue =>
            {
                transform.position = new Vector3(newValue.x, _mobY, newValue.y);
            });

            _viewModel.AnimationDelete.Subscribe(v =>
            {
                if (v)
                {
                    //TODO Анимация удаления объекта После окончания:
                    _viewModel.FinishCurrentAnimation.Value = true;
                }
            });

            _viewModel.State.Subscribe(newState =>
            {
                //TODO Переключаем анимацию от состояния моба.
            });
        }

        public void Update()
        {
            _healthBarBinder.OnUpdate();
         /*   if (_viewModel.CurrentHealth.CurrentValue < _viewModel.MaxHealth)
            {
                _healthBar.gameObject.SetActive(true);
                AlignCamera();
                UpdateParams();
            }
            else
            {
                _healthBar.gameObject.SetActive(false);
            }
*/
            if (transform.rotation != _targetDirection)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, _targetDirection, Time.deltaTime * 10f);
                _healthBarBinder.OnUpdate();
            }
        }

        private Vector3 GetTargetPosition()
        {
            var newValue = _viewModel.RoadPoints[_currentIndexListPoint].Point;
            _targetPosition = new Vector3(newValue.x, _mobY, newValue.y);
            return _targetPosition;
        }
        
        /**
         * Поворачиваем полоску к камере
         */
        private void AlignCamera() {
            if (_viewModel.CameraService.Camera != null) {
                var camXform = _viewModel.CameraService.Camera.transform;
                var forward = _healthBar.transform.position - camXform.position;
                forward.Normalize();
                var up = Vector3.Cross(forward, camXform.right);
                _healthBar.transform.rotation = Quaternion.LookRotation(forward, up);
            }
        }
        
        /**
         * Обновляем шрейдер
         */
        private void UpdateParams() {
            meshRenderer.GetPropertyBlock(matBlock);
            matBlock.SetFloat("_Fill", _viewModel.CurrentHealth.CurrentValue / _viewModel.MaxHealth);
            meshRenderer.SetPropertyBlock(matBlock);
        }
    }
}