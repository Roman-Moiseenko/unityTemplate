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
        
        public void Bind(Sprite sprite, string nameParam, float valueParam, int number, bool isBooster)
        {
            image.GetComponentInChildren<Image>().sprite = sprite;
            nameField.text = nameParam;
            valueField.text = $"{valueParam}";

            transform.localPosition = new Vector3(transform.localPosition.x,
                transform.localPosition.y + 85 * number,
                transform.localPosition.z
            );
        }
    }
}