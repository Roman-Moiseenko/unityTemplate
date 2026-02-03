using System;
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
                var warrior = other.gameObject.GetComponent<WarriorBinder>();
                _coroutine = StartCoroutine(ViewModel.AttackWarrior(warrior.UniqueId));
            }

            if (other.gameObject.CompareTag("Castle"))
            {
                _coroutine = StartCoroutine(ViewModel.AttackCastle());
            }

            if (other.gameObject.CompareTag("Wall"))
            {
                //TODO Наносим урон стене
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Warrior") /* && IsMoving*/)
            {
                //TODO Проверка на other.gameObject.CompareTag("Warrior") и IsMoving ()
                //Исключительный случай, когда 1й воин убит, но моб уже внутри коллайдера 2-го воина    
            }
             
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Warrior") || other.gameObject.CompareTag("Wall"))
            {
                //TODO Продолжаем движение
            }
        }

        private void OnDestroy()
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
        }
    }
}