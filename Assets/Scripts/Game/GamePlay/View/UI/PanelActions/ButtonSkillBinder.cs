using System.Collections.Generic;
using Game.GamePlay.View.Skills;
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
        private SkillViewModel _viewModel;

        //TODO Передать колбек для вызова функции по нажатию и данные по скиллу
        public void Bind(SkillViewModel viewModel)
        {
            _viewModel = viewModel;
            //selected.Find("Icon").transform.SetAsLastSibling();
            selectedIcon.SetAsLastSibling();
            SelectedToggle();
        }

        private void SelectedToggle()
        {
            selectedBackground.gameObject.SetActive(!selectedBackground.gameObject.activeSelf);
            selectedIcon.gameObject.SetActive(!selectedIcon.gameObject.activeSelf);
        }

        private void OnEnable()
        {
            startButton.onClick.AddListener(OnStartSkill);
        }

        private void OnDisable()
        {
            startButton.onClick.AddListener(OnStartSkill);
        }

        private void OnStartSkill()
        {
            _viewModel.StartSkill();
            SelectedToggle();
        }
    }
}