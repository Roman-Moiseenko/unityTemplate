using System.Collections.Generic;
using UnityEngine;

namespace Game.GamePlay.View.UI.PanelBuild
{
    public class LevelBinder : MonoBehaviour
    {
        [SerializeField] private List<RectTransform> stars;

        public void Bind(int currentLevel)
        {
            var index = 0;
            foreach (var star in stars)
            {
                star.gameObject.SetActive(index < currentLevel);
                index++;
            }
        }
    }
}