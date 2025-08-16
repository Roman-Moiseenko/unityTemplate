using System;
using R3;
using TMPro;
using UnityEngine;

namespace Game.GamePlay.View.UI.ScreenGameplay.Popups
{
    public class ReducePopupBinder : MonoBehaviour
    {
        [SerializeField] private Transform _textPanel;
        private Camera _camera;
        private Vector3 _position = Vector3.zero;
        private IDisposable _disposable;

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
        }
        
        private void OnDestroy()
        {
            _disposable.Dispose();
        }

    }
}