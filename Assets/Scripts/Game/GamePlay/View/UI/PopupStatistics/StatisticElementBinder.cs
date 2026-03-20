using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PopupStatistics
{
    public class StatisticElementBinder : MonoBehaviour
    {
        [SerializeField] private Image backgroundCard;
        [SerializeField] private Image iconCard;
        [SerializeField] private Image backgroundDefence;
        [SerializeField] private Image iconDefence;
        [SerializeField] private TMP_Text countEntity;
        
        [SerializeField] private TMP_Text txtPercent;
        [SerializeField] private TMP_Text txtDamage;
        [SerializeField] private Slider sliderPercent;
        [SerializeField] private StatisticsStarsBinder starsBinder;

        public void Bind(StatisticElementViewModel viewModel)
        {
            
        }
    }
}