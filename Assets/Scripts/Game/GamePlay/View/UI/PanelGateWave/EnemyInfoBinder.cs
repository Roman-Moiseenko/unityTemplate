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
        [SerializeField] private Image bossBackground;
        [SerializeField] private Image bossIcon;
        
        public void Bind(Sprite sprite, string nameEnemy, int countEnemy, int number, bool isBoss)
        {
            image.GetComponentInChildren<Image>().sprite = sprite;

            //TODO isBoss
            nameField.text = nameEnemy;
            countField.text = $"x{countEnemy}";
            bossBackground.gameObject.SetActive(isBoss);
            bossIcon.gameObject.SetActive(isBoss);

                //F18C8C
            transform.localPosition = new Vector3(transform.localPosition.x,
                transform.localPosition.y + 110 * number,
                transform.localPosition.z
            );
        }
        
    }
}