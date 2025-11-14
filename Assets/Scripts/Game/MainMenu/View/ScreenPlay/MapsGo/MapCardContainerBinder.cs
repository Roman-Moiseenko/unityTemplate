using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenPlay.MapsGo
{
    public class MapCardContainerBinder : MonoBehaviour
    {
        [SerializeField] private ScrollRect screenView;
        [SerializeField] private RectTransform viewPort;
        [SerializeField] public RectTransform content;
        [SerializeField] private HorizontalLayoutGroup hlg;


        public void Bind()
        {
            
        }
        private void Start()
        {
            //screenView.OnScroll();
        }

        private void Update()
        {

        }
        public void OnScroll2()
        {
            Debug.Log("*****");
        }
        public void OnScroll(BaseEventData eventData)
        {
            //eventData.selectedObject.GetComponent<ScrollRect>()
            Debug.Log(content.localPosition.x);
        }
    }
}