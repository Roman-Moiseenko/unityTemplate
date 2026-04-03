using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelActions
{
    public class ButtonSkillBinder : MonoBehaviour
    {
        [SerializeField] public Button startButton;
        [SerializeField] private Transform selectedBackground;
        [SerializeField] private Transform selectedIcon;
        [SerializeField] private Image imageEpicBackground;
        [SerializeField] private Image imageSkill;
        [SerializeField] private List<Transform> stars;

        //TODO Передать колбек для вызова функции по нажатию и данные по скиллу
        public void Bind()
        {
            //selected.Find("Icon").transform.SetAsLastSibling();
            selectedIcon.SetAsLastSibling();
            SelectedToggle();
        }

        private void SelectedToggle()
        {
            selectedBackground.gameObject.SetActive(!selectedBackground.gameObject.activeSelf);
            selectedIcon.gameObject.SetActive(!selectedIcon.gameObject.activeSelf);
        }
    }
}