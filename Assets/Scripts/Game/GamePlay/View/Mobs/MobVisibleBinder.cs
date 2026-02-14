using System;
using Game.GamePlay.View.Castle;
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
                Debug.Log("В зоне атаки Замка" + ViewModel.UniqueId);
                var castle = other.gameObject.GetComponent<CastleBinder>();
                ViewModel.PullTargets.Add(castle.ViewModel);
            }

            if (other.gameObject.CompareTag("Wall"))
            {
                //TODO Наносим урон стене
                //var wall = other.gameObject.GetComponent<WallBinder>();
                //ViewModel.PullTargets.Add(wall.ViewModel);
            }
        }
        
        
        private void OnDestroy()
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
        }
    }
}