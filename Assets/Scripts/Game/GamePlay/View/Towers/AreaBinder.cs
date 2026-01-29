using System;
using DG.Tweening;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class AreaBinder : MonoBehaviour
    {
        [SerializeField] protected Transform area;
        private bool _isShow;
        
        private void Awake()
        {
            area.transform.localScale = Vector3.zero;
        }

        public void Bind()
        {
            _isShow = false;
            area.transform.localScale = Vector3.zero;
        }

        public void Show(Vector3 dimensions, bool animate = true)
        {
            if (_isShow) return;
            var doScale = SetDimensions(dimensions);
            if (animate)
            {
                area.DOScale(doScale, 0.2f).From(Vector3.zero).SetEase(Ease.OutCirc).SetUpdate(true);
            }
            else
            {
                area.localScale = doScale;
            }
            _isShow = true;
        }

        protected virtual Vector3 SetDimensions(Vector3 radius)
        {
            return radius;
        }

        public void Hide(bool animate = true)
        {
            if (!_isShow) return;
            if (animate)
            {
                area.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InCirc).SetUpdate(true);    
            }
            else
            {
                area.localScale = Vector3.zero;
            }
            
            _isShow = false;
        }

    }
}