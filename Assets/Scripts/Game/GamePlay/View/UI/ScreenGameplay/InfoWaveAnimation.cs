using UnityEngine;

namespace Game.GamePlay.View.UI.ScreenGameplay
{
    public class InfoWaveAnimation : MonoBehaviour
    {
        public void FinishAnimation()
        {
            gameObject.SetActive(false);
            //var parentBinder = gameObject.GetComponent<CurrencyPopupBinder>();// SetActive(false);
           // parentBinder.CurrentState.Value = CurrencyState.Ejection;
            //transform.parent.transform.GetComponent<DamagePopupBinder>().Free.Value = true;
        }
    }
}