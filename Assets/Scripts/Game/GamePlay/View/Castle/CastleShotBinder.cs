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
        private MobEntity _mobEntity;
        private ReactiveProperty<Vector3> _target;
        private bool _isMoving = false;
        private ReactiveProperty<float> _duration; // = new(1f);
        private ReactiveProperty<int> _gameSpeed;// = new(1);
        private float _timeElapsed;
        public ReactiveProperty<bool> IsShotComplete = new(false);

        public void Bind(ReactiveProperty<float> duration, ReactiveProperty<int> gameSpeed)
        {
            _duration = duration;
            _gameSpeed = gameSpeed;
            _beginPosition = transform.localPosition;
            transform.gameObject.SetActive(false);
            
          //  Debug.Log(transform.position + " local = " + transform.localPosition);
        }

        public void FirePrepare(MobEntity mobEntity)
        {
            IsShotComplete.Value = false;
            _target = mobEntity.PositionTarget;
//            _target.Subscribe(v => Debug.Log("Target = " + v));
            transform.localPosition = _beginPosition;
          //  Debug.Log("FirePrepare = " + transform.position + " local = " + transform.localPosition);
        }

        public void Fire(MobEntity mobEntity)
        {
//            Debug.Log("PositionTarget =" + mobEntity.PositionTarget.Value);
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
            if (_gameSpeed.Value == 0) return;
            
            //движение снаряда и поворот
            if (_timeElapsed < _duration.CurrentValue)
            {
                var direction = _target.CurrentValue - transform.position;
                var targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
                
                transform.position = Vector3.Lerp(transform.position, _target.CurrentValue,
                    _timeElapsed / _duration.CurrentValue);
                _timeElapsed += Time.deltaTime;
            }
            else
            {
                Debug.Log("Принудительное попадание");
                FireFinish();
                IsShotComplete.OnNext(true);
            }
        }
        

        private void OnCollisionEnter(Collision other)
        {
            
            if (other.gameObject.CompareTag("Mob"))
            {
                //Debug.Log("OnCollisionEnter = " + other.gameObject.name );
                FireFinish();
                IsShotComplete.OnNext(true);
            }
        }
    }
}