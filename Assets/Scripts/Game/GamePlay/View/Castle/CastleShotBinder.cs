using System.Collections;
using Game.State.Maps.Mobs;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Castle
{
    public class CastleShotBinder : MonoBehaviour
    {
        private Vector3 _position;
        private MobEntity _mobEntity;
        private ReactiveProperty<Vector3> _target;
        private bool _isMoving = false;
        private float _duration;
        private float _timeElapsed;
        public void Bind(Vector3 position)
        {
            _position = position;
        }
        public void FirePrepare(MobEntity mobEntity)
        {
            _target = mobEntity.PositionTarget;
            transform.position = _position;
            var particle = transform.GetComponent<ParticleSystem>();
             if (particle != null) particle.Play(); //Запуск эффекта выстрела
        }
        public void FireStart(float duration)
        {
            //Запуск полета снарядов
            transform.gameObject.SetActive(true);
            _isMoving = true;
            _duration = duration;
           // yield return null;
        }
        private void Update()
        {
            if (!_isMoving) return;
            //TODO движение снаряда и поворот
            if (_timeElapsed < _duration)
            {
                transform.position = Vector3.Lerp(transform.position, _target.CurrentValue, _timeElapsed / _duration);
                _timeElapsed += Time.deltaTime;
            }
            else
            {
                _isMoving = false;
                _timeElapsed = 0f;
            }
        }
        public void FireFinish()
        {
            transform.gameObject.SetActive(false);
        }
        
    }
}