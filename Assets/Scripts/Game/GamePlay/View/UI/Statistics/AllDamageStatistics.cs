using TMPro;
using UnityEngine;

namespace Game.GamePlay.View.UI.Statistics
{
    public class AllDamageStatistics : MonoBehaviour
    {
        [SerializeField] private TMP_Text txtDamage;

        public void Bind(float damage)
        {
            txtDamage.text = damage.ToString();
        }
    }
}