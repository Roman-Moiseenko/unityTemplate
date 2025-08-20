using System;
using Game.GamePlay.View.Towers;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.GamePlay.View.AttackAreas
{
    public class AttackAreaViewModel : IDisposable
    {

        public ReactiveProperty<float> RadiusArea;
        public ReactiveProperty<float> RadiusDisabled;
        public ReactiveProperty<float> RadiusExpansion;
        public ReactiveProperty<Vector2Int> Position;
        private Vector3 _memoryRadius;
        public bool Moving;
        private IDisposable _disposable;
        private Vector2Int _towerPrevious = Vector2Int.zero;
        
        public AttackAreaViewModel(Subject<TowerViewModel> towerClickSubject)
        {
            var d = Disposable.CreateBuilder();
            Position = new ReactiveProperty<Vector2Int>(Vector2Int.zero);
            RadiusArea = new ReactiveProperty<float>(0);
            RadiusDisabled = new ReactiveProperty<float>(0);
            RadiusExpansion = new ReactiveProperty<float>(0);
            _disposable = towerClickSubject.Subscribe(towerViewModel =>
            {
                if (_towerPrevious == towerViewModel.Position.CurrentValue)
                {
                    Hide();
                    _towerPrevious = Vector2Int.zero;
                } else
                {
                    _towerPrevious = towerViewModel.Position.CurrentValue;
                    SetStartPosition(towerViewModel.Position.Value);
                    SetRadius(towerViewModel.GetRadius());
                }
            });
        }

        public void Hide()
        {
            RadiusArea.Value = 0;
            RadiusDisabled.Value = 0;
            RadiusExpansion.Value = 0;
        }

        public void Restore()
        {
            RadiusArea.OnNext(_memoryRadius.x);
            RadiusDisabled.OnNext(_memoryRadius.y);
            RadiusExpansion.OnNext(_memoryRadius.z);
        }

        public void SetRadius(Vector3 radius)
        {
            _memoryRadius = radius;
            RadiusArea.OnNext(radius.x);
            RadiusDisabled.OnNext(radius.y);
            RadiusExpansion.OnNext(radius.z);
        }

        /**
         * Установить позицию без перемещения
         */
        public void SetStartPosition(Vector2Int positionValue)
        {
            Moving = false;
            Position.Value = positionValue;
        }

        /**
         * Переместить в новую точку
         */
        public void SetPosition(Vector2Int positionValue)
        {
            Moving = true;
            Position.Value = positionValue;
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}