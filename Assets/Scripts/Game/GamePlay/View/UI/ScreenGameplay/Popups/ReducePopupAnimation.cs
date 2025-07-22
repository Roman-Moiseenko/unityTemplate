using UnityEngine;

namespace Game.GamePlay.View.UI.ScreenGameplay.Popups
{
    public class ReducePopupAnimation : MonoBehaviour
    {
        public void FinishAnimation()
        {
            transform.parent.transform.gameObject.SetActive(false);
            //transform.parent.transform.GetComponent<DamagePopupBinder>().Free.Value = true;
        }
    }
}