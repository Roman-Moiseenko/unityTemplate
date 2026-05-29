using System;
using Game.GamePlay.View.Castle;
using Game.GamePlay.View.Skills;
using Game.GamePlay.View.Warriors;
using UnityEngine;

namespace Game.GamePlay.View.Mobs
{
    public class MobVisibleBinder : MonoBehaviour
    {
        public MobViewModel ViewModel;
        private Coroutine _coroutine;

        public void Bind(MobViewModel viewModel)
        {
            ViewModel = viewModel;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            // Защита: если ViewModel уже задиспожена, игнорируем столкновение
            if (ViewModel == null) return;
            
            if (other.gameObject.CompareTag("Warrior"))
            {
                var warrior = other.gameObject.GetComponent<WarriorAttackBinder>();
                if (warrior?.ViewModel == null) return;
                //  Debug.Log(warrior.ViewModel.UniqueId + " " + warrior.ViewModel.ConfigId);
                ViewModel.PullTargets.Add(warrior.ViewModel);
                
            }

            if (other.gameObject.CompareTag("Castle"))
            {
//                Debug.Log("В зоне атаки Замка" + ViewModel.UniqueId);
                var castle = other.gameObject.GetComponent<CastleBinder>();
                if (castle?.ViewModel == null) return;
                ViewModel.PullTargets.Add(castle.ViewModel);
            }

            if (other.gameObject.CompareTag("Wall"))
            {
                var wall = other.gameObject.GetComponent<Skill02Binder>();
                //Debug.Log("Wall = " + wall.Duration);
                //TODO Наносим урон стене
                if (wall == null) return;
                ViewModel.PullTargets.Add(wall);
            }
        }
        
        
        private void OnDestroy()
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
        }
    }
}