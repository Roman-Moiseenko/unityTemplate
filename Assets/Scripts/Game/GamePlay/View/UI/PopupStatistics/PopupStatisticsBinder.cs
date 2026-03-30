using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.View.UI.Statistics;
using MVVM.UI;
using TMPro;
using UnityEngine;

namespace Game.GamePlay.View.UI.PopupStatistics
{
    public class PopupStatisticsBinder : PopupBinder<PopupStatisticsViewModel>
    {
        [SerializeField] private AllDamageStatistics allDamageStatistics;
        [SerializeField] private ScrollElementsStatistics scrollElementsStatistics;
        
        protected override void OnBind(PopupStatisticsViewModel viewModel)
        {
            base.OnBind(viewModel);
            allDamageStatistics.Bind(viewModel.AllDamage);
            scrollElementsStatistics.Bind(viewModel.Elements);
        }
        
    }
}