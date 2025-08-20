using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelGateWave
{
    public class UpgradeParameterBinder : MonoBehaviour
    {       
        [SerializeField] private TMP_Text valueField;
        [SerializeField] private Transform image;
        public void Bind(Sprite sprite, int valueParam, int number)
        {
            image.GetComponentInChildren<Image>().sprite = sprite;
            valueField.text = $"{valueParam}";

            transform.localPosition = new Vector3(transform.localPosition.x,
                transform.localPosition.y,
                transform.localPosition.z
            );
        }
    }
}