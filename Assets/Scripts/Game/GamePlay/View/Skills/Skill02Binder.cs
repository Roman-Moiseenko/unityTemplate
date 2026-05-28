using System;
using System.Collections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Skills
{
    public class Skill02Binder : SkillBinder
    {

        protected override void OnBind()
        {
            //Когда выйдет таймер или health = 0 ToDestroy.OnNext(true);
            var vector = new Vector3(ViewModel.EffectDirection.Value.x, 0, ViewModel.EffectDirection.Value.y);
            transform.rotation = Quaternion.LookRotation(vector);
            
            StartCoroutine(TimeCd());
        }

        private IEnumerator TimeCd()
        {
            yield return new WaitForSeconds(10);
            ViewModel.ToDestroy.Value = true;
        }

    }
}