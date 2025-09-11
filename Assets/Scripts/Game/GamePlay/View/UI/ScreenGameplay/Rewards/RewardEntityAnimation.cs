using UnityEngine;

namespace Game.GamePlay.View.UI.ScreenGameplay.Rewards
{
    public class RewardEntityAnimation : MonoBehaviour
    {
        public void FinishAnimation()
        {
            var parentBinder = gameObject.GetComponent<RewardEntityBinder>();// SetActive(false);
            parentBinder.RewardState.Value = CurrencyState.Ejection;
            //transform.parent.transform.GetComponent<DamagePopupBinder>().Free.Value = true;
        }
    }
}