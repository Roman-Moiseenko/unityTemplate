using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.GameRoot.View.Input.TabPanel
{
    [RequireComponent(typeof(Image))]
    public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public TabGroup tabGroup;
        
        public Image background;

        private Vector3 localPositionBase;
        private void Awake()
        {
            localPositionBase = transform.localPosition;
        }

        private void Start()
        {
            background = GetComponent<Image>();
            tabGroup.Subscribe(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tabGroup.OnTabEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
           tabGroup.OnTabExit(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            tabGroup.OnTabSelected(this);
        }

        public void SizeUp()
        {
            transform
                .DOLocalMoveY(transform.localPosition.y + 20, 0.3f)
                .SetEase(Ease.OutQuart)
                .SetUpdate(true);
        }

        public void ResetParams()
        {
            transform.localPosition = localPositionBase;
        }
    }
}