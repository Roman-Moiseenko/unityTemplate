using System.Collections.Generic;
using UnityEngine;

namespace Game.GamePlay.View.UI.PopupStatistics
{

    public class StatisticsStarsBinder : MonoBehaviour
    {
        [SerializeField] private List<RectTransform> stars;

        public void Bind(int viewStars, int allStars = 6)
        {
            var index = 0;
            foreach (var star in stars)
            {
                star.Find("starFill").gameObject.SetActive(index < viewStars);
                star.Find("starEmpty").gameObject.SetActive(index < allStars);
                index++;
            }
        }
    }
}