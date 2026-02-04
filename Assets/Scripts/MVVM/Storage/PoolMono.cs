using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace MVVM.Storage
{
    public class PoolMono<T> where T : MonoBehaviour, IPoolElement
    {
        public T Prefab { get; }
        public bool AutoExpand { get; set; }
        public Transform Container { get; }

        private List<T> _pool;
        //   private int _index;

        public PoolMono(string path, int count, Transform container)
        {
            Prefab = Resources.Load<T>(path);
            Container = container;
            AutoExpand = true;
            CreatePool(count);
        }

        public PoolMono(T prefab, int count, Transform container)
        {
            Prefab = prefab;
            Container = container;
            AutoExpand = true;
            CreatePool(count);
        }

        private void CreatePool(int count)
        {
            _pool = new List<T>();
            for (var i = 0; i < count; i++)
                CreateObject();
        }

        private T CreateObject(bool isActiveByDefault = false)
        {
            var createdObject = Object.Instantiate(Prefab, Container);
            createdObject.gameObject.SetActive(isActiveByDefault);
            createdObject.Bind();
            _pool.Add(createdObject);
            return createdObject;
        }

        public bool HasFreeElement(out T element)
        {
            foreach (var mono in _pool)
            {
                if (!mono.gameObject.activeSelf)
                {
                    element = mono;
                    mono.gameObject.SetActive(false);
                    return true;
                }
            }

            element = null;
            return false;
        }

/*
        private T GetObjectByIndex()
        {
            var element = _pool[_index];
            if (element.gameObject.activeInHierarchy)
            {

            }
            element.gameObject.SetActive(true);
            _index++;
            return element;
        }
*/
        public T GetFreeElement()
        {
            if (HasFreeElement(out var element)) return element;
            
            return CreateObject(false);

            
        }

        //Получить все элементы,

        //Получить только активные элементы
        //Получить кол-во свободных элементов
    }
}