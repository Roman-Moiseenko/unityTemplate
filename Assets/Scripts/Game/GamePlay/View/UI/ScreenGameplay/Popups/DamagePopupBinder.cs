using System;
using DG.Tweening;
using Game.State.Maps.Shots;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.ScreenGameplay.Popups
{
    public class DamagePopupBinder : MonoBehaviour
    {

        [SerializeField] private Transform _textPanel;
        [SerializeField] private TMP_Text text;
        public ReactiveProperty<bool> Free = new();

        private Camera _camera;
        private Vector3 _position = Vector3.zero;
        private IDisposable _disposable;
        private Sequence Sequence;

        public void Bind(Camera camera, Subject<Unit> positionCamera)
        {
            var d = Disposable.CreateBuilder();

            Free.Value = true;
            transform.gameObject.SetActive(false);
            //_animator = _textPanel.GetComponent<Animator>();
            _camera = camera;
            positionCamera.Subscribe(_ =>
            {
                if (Free.Value) return; //Не обсчитываем для свободных блоков
                transform.position = _camera.WorldToScreenPoint(_position);
            }).AddTo(ref d);
            _disposable = d.Build();
        }

        public void StartPopup(Vector3 position, int damage, DamageType damageType)
        {
            _position = position;
            if (_camera == null)
            {
                //TODO Отследить, при проигрыше и закрытии запускается показ урона
                return;
            }
            transform.position = _camera.WorldToScreenPoint(position);
            _textPanel.GetComponent<TMP_Text>().text = damage.ToString();
            _textPanel.GetComponent<TMP_Text>().color = damageType switch
            {
                DamageType.Critical => Color.red,
                DamageType.MassDamage => Color.yellow,
                _ => Color.white
            };
            Free.Value = false;
            transform.gameObject.SetActive(true);
            Sequence = DOTween.Sequence();
            
            Sequence
                .Append(_textPanel
                    .DOLocalMove(new Vector3(0, 14f, 0), 1f)
                    .From(Vector3.zero)
                    .SetUpdate(true)) //Поднимаем
                .Join(_textPanel
                    .DOPunchScale(new Vector3(0.3f, 0.3f, 0), 1f, 0)
                    .SetUpdate(true)) //Масштабируем
                .Join(text
                    .DOFade(0, 1f)
                    .From(1)
                    .SetEase(Ease.InQuad)
                    .SetUpdate(true)) //Плавно исчезаем
                
                .OnComplete(() =>
                {
                    transform.gameObject.SetActive(false);
                    Free.Value = true;
                    Sequence.Kill();
                });
        }
        private void OnDestroy()
        {
            if (Sequence.IsActive())
            {
                Sequence.Kill();
                Sequence = null;
            }
            _disposable.Dispose();
        }
    }
}