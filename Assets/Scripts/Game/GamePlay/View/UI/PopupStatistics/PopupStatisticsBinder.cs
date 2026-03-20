using System.Collections.Generic;
using System.Linq;
using MVVM.UI;
using TMPro;
using UnityEngine;

namespace Game.GamePlay.View.UI.PopupStatistics
{
    public class PopupStatisticsBinder : PopupBinder<PopupStatisticsViewModel>
    {
        [SerializeField] private Transform containerElements;
        [SerializeField] private TMP_Text txtAllDamage;
        
        private List<StatisticElementBinder> _elementBinders = new();
        protected override void OnBind(PopupStatisticsViewModel viewModel)
        {
            base.OnBind(viewModel);
            txtAllDamage.text = viewModel.AllDamage.ToString();
            foreach (var element in viewModel.Elements)
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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var elementBinder in _elementBinders.ToList())
            {
                Destroy(elementBinder.gameObject);
            }
        }
    }
}