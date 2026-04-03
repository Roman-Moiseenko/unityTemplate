using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelActions
{
    public class ButtonHeroBinder : MonoBehaviour
    {
        [SerializeField] public Button startButton;
        //[SerializeField] private Transform selected;
        [SerializeField] private Image imageBackground;
        [SerializeField] private Image imageHero;
        [SerializeField] private List<Transform> stars;

        //TODO Передать колбек для вызова функции по нажатию и данные по скиллу
        public void Bind()
        {
          //  selected.Find("Icon").transform.SetAsFirstSibling();
        }
    }
}