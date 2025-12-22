using System;
using System.Collections;
using Game.Common;
using Game.GamePlay.View.Towers;
using R3;
using Scripts.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.GamePlay.View.AttackAreas
{
    public class AttackAreaViewModel : IDisposable
    {
        public ReactiveProperty<Vector3> Radius;
        public ReactiveProperty<Vector2Int> Position;
        private Vector3 _memoryRadius;
        public bool Moving;
        private IDisposable _disposable;
        private Vector2Int _towerPrevious = Vector2Int.zero;
        public ReactiveProperty<bool> StartAnimationHide = new(false);

        public AttackAreaViewModel(Subject<TowerViewModel> towerClickSubject)
        {
            var d = Disposable.CreateBuilder();
            Position = new ReactiveProperty<Vector2Int>(Vector2Int.zero);
            Radius = new ReactiveProperty<Vector3>(Vector3.zero);
            _disposable = towerClickSubject.Subscribe(towerViewModel =>
            {
                if (_towerPrevious == towerViewModel.Position.CurrentValue)
                {
                    Hide();
                    _towerPrevious = Vector2Int.zero;
                }
                else
                {
                    _towerPrevious = towerViewModel.Position.CurrentValue;
                    SetStartPosition(towerViewModel.Position.Value);
                    SetRadius(towerViewModel.GetRadius());
                }
            });
        }

        public void Hide()
        {
            Radius.Value = Vector3.zero;
        }

        public void Restore()
        {
            Radius.OnNext(_memoryRadius);
        }

        public void SetRadius(Vector3 radius)
        {
            _memoryRadius = radius;
            Radius.OnNext(radius);
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

        public void HideAnimation()
        {
            StartAnimationHide.OnNext(true);
            //var coroutines = GameObject.Find(AppConstants.COROUTINES).GetComponent<Coroutines>();
            //coroutines.StartCoroutine(HideArea());
            //TODO Запуск анимации сжатия до Vector3.zero 
        }
/*
        private IEnumerator HideArea()
        {
            //yield return null;
            while (Radius.Value.x > 0.5)
            {
                Radius.Value = Vector3.Lerp(Radius.Value, Vector3.zero, 0.1f);
                yield return new WaitForSeconds(0.05f);
            }
            Radius.Value = Vector3.zero;
        }*/
    }
}