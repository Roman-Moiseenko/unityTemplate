using System;
using System.Globalization;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.ScreenGameplay.Popups
{
    public class CastleHealthBarBinder : MonoBehaviour
    {
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private Slider _healthProgress;
        private Camera _camera;
        private float _castleFullHealth;
        private IDisposable _disposable;

        public void Bind(Camera camera, Subject<Unit> positionCamera, float castleFullHealth)
        {
            var d = Disposable.CreateBuilder();
            transform.gameObject.SetActive(true);
            _castleFullHealth = castleFullHealth;
            _camera = camera;
            var position = new Vector3(-1f, 1.5f , 0);
            transform.position = _camera.WorldToScreenPoint(position);
            positionCamera.Subscribe(_ =>
            {
                transform.position = _camera.WorldToScreenPoint(position);
            }).AddTo(ref d);
            SetHealth(castleFullHealth);
            _disposable = d.Build();
        }

        public void SetHealth(float health)
        {
            _healthText.text = Mathf.RoundToInt(health).ToString(CultureInfo.CurrentCulture);
            _healthProgress.value = health / _castleFullHealth;
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}