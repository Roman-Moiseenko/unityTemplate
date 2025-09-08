using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GamePlay.View.UI.PanelBuild
{
    public class LevelUpBinder : MonoBehaviour
    {
        [SerializeField] private List<RectTransform> stars;
        private int _animationStart = 0;

        public void Show(int currentLevel)
        {
            var index = 0;
            foreach (var star in stars)
            {
                
                if (index < currentLevel)
                {
                    star.Find("starFill").gameObject.SetActive(true);
                }

                if (index == currentLevel)
                {
                    _animationStart = currentLevel;
                }
                if (index > currentLevel)
                {
                    star.Find("starFill").gameObject.SetActive(false);
                }
                
                index++;
            }
            transform.gameObject.SetActive(true);
            
        }

        private void OnEnable()
        {
            StartCoroutine(ShowStar());
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator ShowStar()
        {
            if (_animationStart == 0) yield break; 
            
            stars[_animationStart].Find("starFill").gameObject.SetActive(true);
            yield return new WaitForSeconds(0.4f);
            stars[_animationStart].Find("starFill").gameObject.SetActive(false);
            yield return new WaitForSeconds(0.4f);
            StartCoroutine(ShowStar());
        }
        

        public void Hide()
        {
            gameObject.SetActive(false);
            _animationStart = 0;
            StopCoroutine(ShowStar());
        }
        
    }
}