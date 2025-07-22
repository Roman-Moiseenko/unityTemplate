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
        

        public void Bind(Camera camera, Subject<Unit> positionCamera, float castleFullHealth)
        {
            transform.gameObject.SetActive(true);
            _castleFullHealth = castleFullHealth;
            _camera = camera;
            var position = new Vector3(-1f, 1.5f, 0);
            positionCamera.Subscribe(_ =>
            {
                transform.position = _camera.WorldToScreenPoint(position);
            });
            SetHealth(castleFullHealth);
        }

        public void SetHealth(float health)
        {
            _healthText.text = health.ToString(CultureInfo.CurrentCulture);
            _healthProgress.value = health / _castleFullHealth;
        }
    }
}