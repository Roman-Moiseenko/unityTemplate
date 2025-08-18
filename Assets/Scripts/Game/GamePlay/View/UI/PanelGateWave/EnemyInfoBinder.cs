using System;
using Game.State.Maps.Mobs;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.GamePlay.View.UI.PanelGateWave
{
    public class EnemyInfoBinder : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameField;
        [SerializeField] private TMP_Text countField;
        [SerializeField] private Transform fast;
        [SerializeField] private Transform elemental;
        [SerializeField] private Transform advanced;
        [SerializeField] private Transform support;
        
        
        public void Bind(MobDefence defenceEnemy, string nameEnemy, int countEnemy, int number)
        {
            fast.gameObject.SetActive(false);
            elemental.gameObject.SetActive(false);
            advanced.gameObject.SetActive(false);
            support.gameObject.SetActive(false);

            switch (defenceEnemy)
            {
                case MobDefence.Fast: fast.gameObject.SetActive(true); break;
                case MobDefence.Elemental: elemental.gameObject.SetActive(true); break;
                case MobDefence.Advanced: advanced.gameObject.SetActive(true); break;
                case MobDefence.Support: support.gameObject.SetActive(true); break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(defenceEnemy), defenceEnemy, null);
            }
            //transform.Find("Image").GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>($"Images/Mobs/Defence/{defenceEnemy}");
            nameField.text = nameEnemy;
            countField.text = $"x{countEnemy}";

            transform.localPosition = new Vector3(transform.localPosition.x,
                transform.localPosition.y + 155 * number,
                transform.localPosition.z
            );
        }
        
        
    }
}