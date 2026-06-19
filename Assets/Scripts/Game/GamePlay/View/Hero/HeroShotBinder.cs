using Game.GamePlay.View.Castle;
using Game.GamePlay.View.Mobs;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Hero
{
    public class HeroShotBinder : MonoBehaviour
    {
        private Vector3 _beginPosition;
        private MobViewModel _mobEntity;
        private ReactiveProperty<Vector3> _target;
        private bool _isMoving = false;
        private float _duration;
        private float _timeElapsed;
        public ReactiveProperty<bool> IsShotComplete = new(false);

        public void Bind(HeroViewModel viewModel)
        {
            _duration = viewModel.Speed.Value;
            _beginPosition = transform.localPosition;
            transform.gameObject.SetActive(false);
        }

        public void FirePrepare(MobViewModel mobViewModel)
        {
            IsShotComplete.Value = false;
            _target = mobViewModel.PositionTargetForShot;
            transform.localPosition = _beginPosition;
        }

        public void Fire()
        {
            transform.gameObject.SetActive(true);
            _timeElapsed = 0f;
            _isMoving = true;
        }

        private void FireFinish()
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