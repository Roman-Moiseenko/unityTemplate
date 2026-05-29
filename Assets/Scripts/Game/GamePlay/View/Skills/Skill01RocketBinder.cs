using System.Collections;
using Game.GamePlay.View.Towers;
using UnityEngine;

namespace Game.GamePlay.View.Skills
{
    public class Skill01RocketBinder : MonoBehaviour
    {
        [SerializeField] private ExplosionBinder explosion;
        [SerializeField] private ParticleSystem trace;
        [SerializeField] private Transform missile;

        internal const float StartY = 15f;
        internal const float EndY = 0f;
        internal const float Speed = 50f;
        private const float ExplosionDuration = 0.5f;

        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private Coroutine _rocketLoop;

        public void Bind()
        {
            _startPosition = new Vector3(transform.position.x, StartY, transform.position.z);
            _targetPosition = new Vector3(transform.position.x, EndY, transform.position.z);
            
            HideRocket();
        }

        private void HideRocket()
        {
            transform.position = _startPosition;
            missile.gameObject.SetActive(false);
            if (trace != null)
            {
                trace.Stop();
                trace.gameObject.SetActive(false);
            }
        }

        private void ShowRocket()
        {
            transform.position = _startPosition;
            missile.gameObject.SetActive(true);
            if (trace != null)
            {
                trace.gameObject.SetActive(true);
                trace.Play();
            }
        }

        public void StartLoop()
        {
            _rocketLoop = StartCoroutine(RocketLoop());
        }

        private IEnumerator RocketLoop()
        {
            while (true)
            {
                ShowRocket();

                // Move rocket from Y=15 to Y=0
                while (Vector3.Distance(transform.position, _targetPosition) > 0.01f)
                {
                    transform.position = Vector3.MoveTowards(
                        transform.position,
                        _targetPosition,
                        Speed * Time.deltaTime);
                    yield return null;
                }

                // Reached Y=0 - trigger explosion
                if (explosion != null)
                {
                    explosion.Play(Vector3.zero);
                }

                // Hide rocket and trail during explosion
                HideRocket();

                // Wait for explosion duration
                yield return new WaitForSeconds(ExplosionDuration);

                // Loop: rocket automatically returns to Y=15 and repeats
            }
        }

        private void OnDestroy()
        {
            if (_rocketLoop != null)
            {
                StopCoroutine(_rocketLoop);
            }
        }
    }
}
