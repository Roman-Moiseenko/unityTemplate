using System.Collections;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.GamePlay.View.UI.ScreenGameplay.Rewards
{
    public class CurrencyPopupBinder : MonoBehaviour
    {
        private Camera _camera;
        public ReactiveProperty<bool> Free;
        public Animator animator;
        public ReactiveProperty<CurrencyState> CurrentState;
        private Vector3 _target;

        
        public void Bind(Camera camera, Subject<Unit> positionCamera, Vector3 targetFinish)
        {
            Free = new ReactiveProperty<bool>(true);
            CurrentState = new ReactiveProperty<CurrencyState>(CurrencyState.Rest);
            transform.gameObject.SetActive(false);
            _camera = camera;
            animator = gameObject.GetComponent<Animator>();
            animator.enabled = false;
           // _isMoving = false;

            CurrentState.Skip(1).Subscribe(state =>
            {
                if (state == CurrencyState.Animation)
                {
                    animator.enabled = true;
                }

                if (state == CurrencyState.Ejection)
                {
                    animator.enabled = false;
                    var random = Random.insideUnitSphere;
                    var target = new Vector3(
                        transform.position.x + random.x * 100,
                        transform.position.y + random.y * 100,
                        transform.position.z
                        );
                    StartCoroutine(Ejection(target));
                }

                if (state == CurrencyState.Delay)
                {
                    StartCoroutine(DelayMoving());
                }

                if (state == CurrencyState.Moving)
                {
                    StartCoroutine(Moving(targetFinish));
                }
                if (state == CurrencyState.Rest)
                {
                    transform.gameObject.SetActive(false);
                    Free.Value = true;
                }
            });
        }
        
        
        public void StartPopup(Vector3 position)
        {
            transform.position = _camera.WorldToScreenPoint(position);
            transform.gameObject.SetActive(true);
            Free.Value = false;
            CurrentState.Value = CurrencyState.Animation;
        }
        
        private IEnumerator DelayMoving()
        {
            yield return new WaitForSeconds(0.5f);
            CurrentState.Value = CurrencyState.Moving;
        }

        private IEnumerator Moving(Vector3 target)
        {
            float timeElapsed = 0;
            var duration = 0.7f;
            var startPosition = transform.position;
            var fading = false; 
            
            while (timeElapsed < duration)
            {
                if (timeElapsed > duration / 0.8) fading = true;
                var delta = !fading ? Time.deltaTime : Time.deltaTime / 2;
                transform.position = Vector3.Lerp(startPosition, target, timeElapsed / duration);
                timeElapsed += delta;
                yield return null;
            }
            transform.position = target;
            CurrentState.Value = CurrencyState.Rest;
        }
        
        private IEnumerator Ejection(Vector3 target)
        {
            float timeElapsed = 0;
            float duration = 0.8f;
            
            while (timeElapsed < duration)
            {
                transform.position = Vector3.Lerp(transform.position, target, timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = target;
            CurrentState.Value = CurrencyState.Delay;
        }
    }
}