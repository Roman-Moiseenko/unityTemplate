using Game.State.Maps.Shots;
using R3;
using TMPro;
using UnityEngine;

namespace Game.GamePlay.View.Damages
{
    public class DamagePopupBinder : MonoBehaviour
    {

        [SerializeField] private Transform _textPanel;
        public ReactiveProperty<bool> Free = new();
        //private Animator _animator;
        private Camera _camera;
        private Vector3 _position = Vector3.zero;
        
        public void Bind(Camera camera, Subject<Unit> positionCamera)
        {
            Free.Value = true;
            transform.gameObject.SetActive(false);
            //_animator = _textPanel.GetComponent<Animator>();
            _camera = camera;
            positionCamera.Subscribe(_ => transform.position = _camera.WorldToScreenPoint(_position));
        }

        public void StartPopup(Vector3 position, int damage, DamageType damageType)
        {
            _position = position;
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
        
        /**
         * Функция привязана к _textPanel
         */
        public void FinishAnimation()
        {
            transform.parent.transform.gameObject.SetActive(false);
            transform.parent.transform.GetComponent<DamagePopupBinder>().Free.Value = true;
            
        }
        
    }
}