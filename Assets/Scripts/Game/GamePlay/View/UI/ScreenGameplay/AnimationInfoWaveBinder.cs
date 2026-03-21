using System;
using DG.Tweening;
using UnityEngine;

namespace Game.GamePlay.View.UI.ScreenGameplay
{
    public class AnimationInfoWaveBinder : MonoBehaviour
    {
        [SerializeField] private Transform leftFirstArrow;
        [SerializeField] private Transform leftSecondArrow;
        [SerializeField] private Transform rightFirstArrow;
        [SerializeField] private Transform rightSecondArrow;

        private float _leftFirstX;
        private float _leftSecondX;
        private float _rightFirstX;
        private float _rightSecondX;

        private const int DELTA_X = 50;
        private const float DELTA_SCALE_FIRST = 0.8f;
        private const float DELTA_SCALE_SECOND = 0.6f;
        private const float SCALE_FIRST = 1f;
        private const float SCALE_SECOND = 0.8f;
        private const float DURATION = 0.3f;
        public Sequence Sequence { get; set; }

        private void Awake()
        {
            gameObject.SetActive(false);
            _leftFirstX = leftFirstArrow.position.x;
            _leftSecondX = leftSecondArrow.position.x;
            _rightFirstX = rightFirstArrow.position.x;
            _rightSecondX = rightSecondArrow.position.x;
            Debug.Log(_leftFirstX + " " + _leftSecondX + " " + _rightFirstX + " " + _rightSecondX);
        }

        public void Bind()
        {
        }

        public void Start()
        {
            gameObject.SetActive(true);
            Sequence = DOTween.Sequence();
            Sequence
                .Append(DOTween.Sequence()
                    .Join(leftSecondArrow.DOMoveX(_leftSecondX - DELTA_X, DURATION).From(_leftSecondX))
                    .Join(leftSecondArrow.DOScale(DELTA_SCALE_SECOND, DURATION).From(SCALE_SECOND))
                    .Join(rightSecondArrow.DOMoveX(_rightSecondX + DELTA_X, DURATION).From(_rightSecondX))
                    .Join(rightSecondArrow.DOScale(DELTA_SCALE_SECOND, DURATION).From(SCALE_SECOND))
                )
                .Append(DOTween.Sequence()
                    .Join(leftFirstArrow.DOMoveX(_leftFirstX - DELTA_X, DURATION).From(_leftFirstX))
                    .Join(leftFirstArrow.DOScale(DELTA_SCALE_FIRST, DURATION).From(SCALE_FIRST))
                    .Join(rightFirstArrow.DOMoveX(_rightFirstX + DELTA_X, DURATION).From(_rightFirstX))
                    .Join(rightFirstArrow.DOScale(DELTA_SCALE_FIRST, DURATION).From(SCALE_FIRST))
                )
                .AppendInterval(0.2f)
                .SetLoops(2)
                .OnComplete(() =>
                {
                    Sequence.Kill();
                    gameObject.SetActive(false);
                });

            //Animation
        }
    }
}