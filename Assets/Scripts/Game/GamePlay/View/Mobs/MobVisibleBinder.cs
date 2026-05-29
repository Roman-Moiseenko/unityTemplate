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
            if (other.gameObject.CompareTag("Warrior"))
            {
                var warrior = other.gameObject.GetComponent<WarriorAttackBinder>();
              //  Debug.Log(warrior.ViewModel.UniqueId + " " + warrior.ViewModel.ConfigId);
                ViewModel.PullTargets.Add(warrior.ViewModel);
                
            }

            if (other.gameObject.CompareTag("Castle"))
            {
//                Debug.Log("В зоне атаки Замка" + ViewModel.UniqueId);
                var castle = other.gameObject.GetComponent<CastleBinder>();
                ViewModel.PullTargets.Add(castle.ViewModel);
            }

            if (other.gameObject.CompareTag("Wall"))
            {
                var wall = other.gameObject.GetComponent<Skill02Binder>();
                //Debug.Log("Wall = " + wall.Duration);
                //TODO Наносим урон стене
                
                ViewModel.PullTargets.Add(wall);
            }
        }
        
        
        private void OnDestroy()
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
        }
    }
}