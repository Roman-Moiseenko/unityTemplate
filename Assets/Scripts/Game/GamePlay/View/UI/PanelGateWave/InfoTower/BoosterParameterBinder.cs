using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelGateWave.InfoTower
{
    public class BoosterParameterBinder : MonoBehaviour
    {       
        [SerializeField] private TMP_Text valueField;
        [SerializeField] private Transform image;
        public void Bind(Sprite sprite, float valueParam)
        {
            image.GetComponentInChildren<Image>().sprite = sprite;
            valueField.text = $"{valueParam}%";

            transform.localPosition = new Vector3(transform.localPosition.x,
                transform.localPosition.y,
                transform.localPosition.z
            );
        }
    }
}