using System;
using Game.State.Maps.Mobs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelGateWave
{
    public class EnemyInfoBinder : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameField;
        [SerializeField] private TMP_Text countField;
        [SerializeField] private Transform image;
        
        public void Bind(Sprite sprite, string nameEnemy, int countEnemy, int number)
        {
            image.GetComponentInChildren<Image>().sprite = sprite;

            nameField.text = nameEnemy;
            countField.text = $"x{countEnemy}";

            transform.localPosition = new Vector3(transform.localPosition.x,
                transform.localPosition.y + 110 * number,
                transform.localPosition.z
            );
        }
        
    }
}