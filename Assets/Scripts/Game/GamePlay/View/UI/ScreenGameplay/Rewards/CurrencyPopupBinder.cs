using System;
using System.Collections;
using DG.Tweening;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.GamePlay.View.UI.ScreenGameplay.Rewards
{
    public class CurrencyPopupBinder : MonoBehaviour
    {
        private Camera _camera;
        public ReactiveProperty<bool> Free;
       // public Animator animator;

        private Vector3 _target;
        private Vector3 _targetFinish;
        private Sequence Sequence { get; set; }

        public void Bind(Camera camera, Subject<Unit> positionCamera, Vector3 targetFinish)
        {
            Free = new ReactiveProperty<bool>(true);
            transform.gameObject.SetActive(false);
            _camera = camera;
            _targetFinish = targetFinish;
            //animator = gameObject.GetComponent<Animator>();
//            animator.enabled = false;
        }
        
        public void StartPopup(Vector3 position)
        {
            transform.position = _camera.WorldToScreenPoint(position);
            transform.gameObject.SetActive(true);
            Free.Value = false;

            var random = Random.insideUnitSphere;
            var targetEjection = new Vector3(
                transform.position.x + random.x * 100,
                transform.position.y + random.y * 100,
                transform.position.z
            );
            Sequence = DOTween.Sequence();
            Sequence
                .Append(
                    transform.GetComponent<RectTransform>()
                        .DOSizeDelta(new Vector2(50, 50), 0.4f)
                        .From(new Vector2(25, 25))
                        .SetEase(Ease.OutQuint))
                .Join(
                    transform
                        .DOMove(targetEjection, 0.4f)
                        .SetEase(Ease.OutQuint))
                .SetDelay(0.1f)
                .Append(
                    transform
                        .DOMove(_targetFinish, 0.7f)
                        .SetEase(Ease.InOutCirc))
                .OnComplete(() =>
                {
                    transform.gameObject.SetActive(false);
                    Free.Value = true;
                    Sequence.Kill();
                });
        }


        private void OnDestroy()
        {
            Sequence.Kill();
        }
    }
}