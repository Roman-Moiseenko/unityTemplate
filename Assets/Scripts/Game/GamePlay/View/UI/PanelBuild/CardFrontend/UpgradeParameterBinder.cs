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
            textValue.text = $"{value}%";
            transform.gameObject.SetActive(true);
        }
    }
}