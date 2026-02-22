using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelBuild.CardFrontend
{
    public class UpgradeArrowBinder : MonoBehaviour
    {
        private Sequence sequence;
        private const float Speed = 0.5f;
        private const float Move = 10;
        private const float Scale = 1.25f;
        public void Bind()
        {
            var position = transform.localPosition;
            var originalScale = transform.localScale;
//            Debug.Log(transform.localPosition);
            var image = transform.GetComponent<Image>();
            var originalColor = image.color;
            
            sequence = DOTween.Sequence();
            sequence
                .Append(transform
                    .DOLocalMoveY(Move + position.y, Speed)
                    .From(position.y)
                    .SetEase(Ease.Linear)
                    .SetUpdate(true))
                .Join(
                    transform
                        .DOScale(Scale, Speed)
                        .From(1)
                        .SetEase(Ease.Linear)
                        .SetUpdate(true)
                    )
                .Join(image
                    .DOFade(0.75f, Speed)
                    .From(0.0f)
                    .SetEase(Ease.Linear)
                    .SetUpdate(true)
                )
                .Append(transform
                    .DOLocalMoveY(Move * 2 + position.y, Speed)
                    .From(Move + position.y)
                    .SetEase(Ease.Linear)
                    .SetUpdate(true))
                .Join(transform
                    .DOScale(1f, Speed)
                    .From(Scale)
                    .SetEase(Ease.Linear)
                    .SetUpdate(true)
                )
                .Join(image
                    .DOFade(0.0f, Speed)
                    .SetEase(Ease.Linear)
                    .SetUpdate(true)
                )
                
                .AppendCallback(() =>
                {
                    transform.gameObject.SetActive(false);
                })
                .SetDelay(0.2f)
                .AppendCallback(() =>
                {
                    transform.localPosition = position;
                    transform.localScale = originalScale;
                    image.color = originalColor;
                    transform.gameObject.SetActive(true);
                }).SetUpdate(true).SetLoops(-1);
        }

      /*  private void OnEnable()
        {
            sequence.Play();
        }

        private void OnDisable()
        {
            sequence?.Pause();
        }*/

        private void OnDestroy()
        {
            sequence?.Kill();
        }
    }
}