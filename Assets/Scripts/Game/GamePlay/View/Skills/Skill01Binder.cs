using System;
using System.Collections;
using R3;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.GamePlay.View.Skills
{
    public class Skill01Binder : SkillBinder
    {
        

        protected override void OnBind()
        {
            //Когда выйдет таймер или health = 0 ToDestroy.OnNext(true); 
            StartCoroutine(TimeCd());
        }

        private IEnumerator TimeCd()
        {
            yield return new WaitForSeconds(10);
            ViewModel.ToDestroy.Value = true;
        }

    }
}