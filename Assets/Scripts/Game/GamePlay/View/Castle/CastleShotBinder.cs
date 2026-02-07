using System;
using System.Collections;
using Game.GamePlay.View.Mobs;
using Game.State.Maps.Mobs;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Castle
{
    public class CastleShotBinder : MonoBehaviour
    {
        private Vector3 _beginPosition;
        private MobViewModel _mobEntity;
        private ReactiveProperty<Vector3> _target;
        private bool _isMoving = false;
        private float _duration;
        private float _timeElapsed;
        public ReactiveProperty<bool> IsShotComplete = new(false);

        public void Bind(CastleViewModel viewModel)
        {
            _duration = viewModel.CastleEntity.Speed;
            _beginPosition = transform.localPosition;
            transform.gameObject.SetActive(false);
        }

        public void FirePrepare(MobViewModel mobViewModel)
        {
            IsShotComplete.Value = false;
            _target = mobViewModel.PositionTarget;
            transform.localPosition = _beginPosition;
        }

        public void Fire()
        {
            transform.gameObject.SetActive(true);
            _timeElapsed = 0f;
            _isMoving = true;
        }

        public void FireFinish()
        {
            _isMoving = false;
            _timeElapsed = 0f;
            transform.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!_isMoving) return;
            //движение снаряда и поворот
            if (_timeElapsed < _duration)
            {
                var direction = _target.CurrentValue - transform.position;
                if (direction != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(direction);   
                
                transform.position = Vector3.Lerp(transform.position, _target.CurrentValue,
                    _timeElapsed / _duration);
                _timeElapsed += Time.deltaTime;
            }
            else
            {
//                Debug.Log("Принудительное попадание");
                FireFinish();
                IsShotComplete.OnNext(true);
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Mob"))
            {
                FireFinish();
                IsShotComplete.OnNext(true);
            }
        }
    }
}