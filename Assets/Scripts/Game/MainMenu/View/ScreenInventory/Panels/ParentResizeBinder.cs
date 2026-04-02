using System;
using System.Collections.Generic;
using System.Linq;
using Game.MainMenu.View.Common;
using R3;
using UnityEngine;

namespace Game.MainMenu.View.ScreenInventory.Panels
{
    /**
     * Класс изменяющий размер в зависимости от дочерних,
     * и отлавливает события изменения размера из дочерних объектов ResizeSubject
     */
    [RequireComponent(typeof(ResizeSubject))] //Класс передает в родительский объект события
    public class ParentResizeBinder : MonoBehaviour
    {
        private List<IDisposable> _disposables = new(); 
        private ResizeSubject _subject;

        private void Awake()
        {
            _subject = gameObject.GetComponent<ResizeSubject>();
        }
        
        private void OnEnable()
        {
            foreach (Transform child in transform)
            {
                var resizeBinder = child.GetComponent<ResizeSubject>();
                //Если есть дочерние изменяемые компоненты, подписываемся на событие ResizeSubject
                if (resizeBinder != null)
                {
                    var d = resizeBinder.OnResize.Subscribe(_ => ResizeContainer());
                    _disposables.Add(d);
                }
            }
            //Возможно лишние
            ResizeContainer();
        }

        private void OnDestroy()
        {
            foreach (var disposable in _disposables.ToList())
            {
                disposable.Dispose();
                _disposables.Remove(disposable);
            }
        }


        private void ResizeContainer()
        {
            var sizeDelta = transform.GetComponent<RectTransform>().sizeDelta;
            sizeDelta.y = 0;
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                {
                    sizeDelta.y += child.GetComponent<RectTransform>().sizeDelta.y;
                }
            }
            transform.GetComponent<RectTransform>().sizeDelta =  sizeDelta;
            _subject.OnResize.OnNext(Unit.Default); //Передаем дальше наверх событие изменения размера
        }
    }
}