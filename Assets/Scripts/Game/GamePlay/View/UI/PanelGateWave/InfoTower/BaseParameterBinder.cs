using System;
using Game.State.Maps.Towers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelGateWave.InfoTower
{
    public class BaseParameterBinder : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameField;
        [SerializeField] private TMP_Text valueField;
        [SerializeField] private Transform image;
        public string colorBoss = "FF5353";
        public string colorBooster = "#487F1E";
        
        public void Bind(Sprite sprite, TowerParameterType parameter, Vector2 valueParam)
        {
            image.GetComponentInChildren<Image>().sprite = sprite;
            nameField.text = parameter.GetString();

            var value = Math.Round(valueParam.x + valueParam.y, 1);
            valueField.text = $"{value}{parameter.GetMeasure()}" ;
            if (valueParam.y > 0)
            {
                if (ColorUtility.TryParseHtmlString(colorBooster, out var newColor))
                    valueField.color = newColor;    
            }
            else
            {
                valueField.color = Color.white;
            }
            
        }
    }
}