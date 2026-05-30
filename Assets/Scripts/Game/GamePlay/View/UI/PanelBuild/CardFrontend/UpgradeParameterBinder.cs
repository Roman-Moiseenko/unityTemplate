using Game.State.Maps.Skills;
using Game.State.Maps.Towers;
using TMPro;
using UnityEngine;

namespace Game.GamePlay.View.UI.PanelBuild.CardFrontend
{
    public class UpgradeParameterBinder : MonoBehaviour
    {
        [SerializeField] private TMP_Text textValue;
        [SerializeField] private TMP_Text textName;

        public void Bind(float value, TowerParameterType parameter)
        {
            textName.text = parameter.GetString();
            _Bind(value);
        }
        public void Bind(float value, SkillParameterType parameter)
        {
            textName.text = parameter.GetString();
            _Bind(value);
        }

        private void _Bind(float value)
        {
            textValue.text = $"{value}%";
            transform.gameObject.SetActive(true);
        }
        
        //SkillParameterType
    }
}