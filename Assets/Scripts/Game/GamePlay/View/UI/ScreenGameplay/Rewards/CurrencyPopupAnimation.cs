using UnityEngine;

namespace Game.GamePlay.View.UI.ScreenGameplay.Rewards
{
    public class CurrencyPopupAnimation : MonoBehaviour
    {
        public void FinishAnimation()
        {
            var parentBinder = gameObject.GetComponent<CurrencyPopupBinder>();// SetActive(false);
            parentBinder.CurrentState.Value = CurrencyState.Ejection;
            //transform.parent.transform.GetComponent<DamagePopupBinder>().Free.Value = true;
        }
    }
}