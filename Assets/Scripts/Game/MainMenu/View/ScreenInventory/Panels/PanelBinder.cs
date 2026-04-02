using System;
using Game.MainMenu.View.Common;
using R3;
using UnityEngine;

namespace Game.MainMenu.View.ScreenInventory.Panels
{
    /**
     * Общий класс для панелей, добавляет в каждый экземпляр ResizeSubject
     * и содержит общие универсальные методы изменения высоты
     */
    [RequireComponent(typeof(ResizeSubject))] //Класс передает в родительский объект события
    public abstract class PanelBinder : MonoBehaviour
    {
        private ResizeSubject _subject;

        private void Awake()
        {
            _subject = gameObject.GetComponent<ResizeSubject>();
        }

        protected virtual void OnEnable()
        {
            //При активации панели вызываем событие изменения размеров
            _subject.OnResize.OnNext(Unit.Default);
        }

        private void ResizeContainer()
        {
            //Меняем размеры общего transform
            var sizeDelta = transform.GetComponent<RectTransform>().sizeDelta;
            sizeDelta.y = 0;
            foreach (Transform child in transform)
            {
                sizeDelta.y += child.GetComponent<RectTransform>().sizeDelta.y;
            }

            transform.GetComponent<RectTransform>().sizeDelta = sizeDelta;
            //Передаем наверх, что размеры изменились
            _subject.OnResize.OnNext(Unit.Default);
        }

        /**
         * Универсальная функция изменения размера контейнера с Grid
         */
        protected void UpdateContainer(RectTransform container, int count, ContainerConsts consts)
        {
            var sizeDelta = container.sizeDelta;
            var rows = Math.Ceiling(count / (consts.Cols * 1f));

            sizeDelta.y = count == 0 ? 0 : (float)(rows * consts.Height + (rows - 1) * consts.Spacing);
            sizeDelta.y += consts.MinHeight;
            container.sizeDelta = sizeDelta;
            ResizeContainer();
        }
    }
}