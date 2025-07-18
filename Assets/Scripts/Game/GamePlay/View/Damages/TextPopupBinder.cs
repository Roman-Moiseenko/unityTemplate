using UnityEngine;

namespace Game.GamePlay.View.Damages
{
    public class TextPopupBinder : MonoBehaviour
    {
        public void FinishAnimation()
        {
            transform.parent.transform.gameObject.SetActive(false);
            transform.parent.transform.GetComponent<DamagePopupBinder>().Free.Value = true;
        }
    }
}