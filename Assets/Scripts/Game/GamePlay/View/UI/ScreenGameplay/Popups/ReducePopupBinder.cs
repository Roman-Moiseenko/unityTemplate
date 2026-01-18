using System;
using DG.Tweening;
using R3;
using TMPro;
using UnityEngine;

namespace Game.GamePlay.View.UI.ScreenGameplay.Popups
{
    public class ReducePopupBinder : MonoBehaviour
    {
        [SerializeField] private Transform _textPanel;
        [SerializeField] private TMP_Text text;
        private Camera _camera;
        private Vector3 _position = Vector3.zero;
        private IDisposable _disposable;
        private Sequence Sequence;
        
        public void Bind(Camera camera, Subject<Unit> positionCamera)
        {
            var d = Disposable.CreateBuilder();

            transform.gameObject.SetActive(false);
            //_animator = _textPanel.GetComponent<Animator>();
            _camera = camera;
            positionCamera.Subscribe(_ =>
            {
                transform.position = _camera.WorldToScreenPoint(_position);
            }).AddTo(ref d);
            _disposable = d.Build();
        }
        
        
        public void StartPopup(Vector3 position, float reduce)
        {
            _position = position;
            transform.position = _camera.WorldToScreenPoint(position);
            _textPanel.GetComponent<TMP_Text>().text = reduce.ToString();
            
            transform.gameObject.SetActive(true);
            Sequence = DOTween.Sequence();

            Sequence
                .Append(_textPanel
                    .DOLocalMove(new Vector3(0, 60, 0), 0.4f)
                    .From(Vector3.zero)
                    .SetUpdate(true)) //Поднимаем
                .Join(text
                    .DOFade(0, 0.3f)
                    .From(1)
                    .SetEase(Ease.InQuad)
                    .SetUpdate(true)) //Плавно исчезаем
                
                .OnComplete(() =>
                {
                    transform.gameObject.SetActive(false);
                    Sequence.Kill();
                });
            
        }
        
        private void OnDestroy()
        {
            _disposable.Dispose();
        }

    }
}