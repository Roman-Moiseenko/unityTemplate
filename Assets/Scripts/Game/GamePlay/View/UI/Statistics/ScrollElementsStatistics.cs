using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.View.UI.PopupStatistics;
using UnityEngine;

namespace Game.GamePlay.View.UI.Statistics
{
    public class ScrollElementsStatistics : MonoBehaviour
    {
        [SerializeField] private Transform containerElements;

        private List<StatisticElementBinder> _elementBinders = new();

        public void Bind(List<StatisticElementViewModel> elements)
        {
            foreach (var element in elements)
            {
                CreateElement(element);
            }
        }
        
        private void CreateElement(StatisticElementViewModel element)
        {
            var prefabPath = $"Prefabs/UI/Gameplay/Popups/StatisticElement"; //Перенести в настройки уровня
            var elementPrefab = Resources.Load<StatisticElementBinder>(prefabPath);
            var createdElement = Instantiate(elementPrefab, containerElements);
            createdElement.Bind(element);
            _elementBinders.Add(createdElement);
        }

        protected void OnDestroy()
        {
            foreach (var elementBinder in _elementBinders.ToList())
            {
                Destroy(elementBinder.gameObject);
            }
        }
    }
}