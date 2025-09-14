using System;
using Game.State.Maps.Shots;
using R3;
using TMPro;
using UnityEngine;

namespace Game.GamePlay.View.UI.ScreenGameplay.Popups
{
    public class DamagePopupBinder : MonoBehaviour
    {

        [SerializeField] private Transform _textPanel;
        public ReactiveProperty<bool> Free = new();
        //private Animator _animator;
        private Camera _camera;
        private Vector3 _position = Vector3.zero;
        private IDisposable _disposable;

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
        }
        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}