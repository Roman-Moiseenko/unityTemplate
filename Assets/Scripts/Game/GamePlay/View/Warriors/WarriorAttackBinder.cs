using System;
using Game.GamePlay.Fsm.WarriorStates;
using Game.GamePlay.View.Mobs;
using UnityEngine;

namespace Game.GamePlay.View.Warriors
{
    public class WarriorAttackBinder : MonoBehaviour
    {
        public WarriorViewModel ViewModel;
        
        public void Bind(WarriorViewModel viewModel)
        {
            ViewModel = viewModel;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!ViewModel.FsmWarrior.IsGoToMob()) return;
            if (!other.gameObject.CompareTag("MobVisible")) return;
            //Включаем только тогда, когда "шли к мобу"
            var mobBinder = other.gameObject.GetComponent<MobVisibleBinder>();
            ViewModel.PullAttacks.Add(mobBinder.ViewModel);
            //Атакуем моба, который в цели
//            if (mobBinder.ViewModel.UniqueId == ViewModel.MobTarget.CurrentValue.UniqueId)
                ViewModel.FsmWarrior.Fsm.SetState<FsmWarriorAttack>(mobBinder.ViewModel); 
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("MobVisible")) return;
            var mobBinder = other.gameObject.GetComponent<MobVisibleBinder>();
            ViewModel.PullAttacks.Remove(mobBinder.ViewModel); //Если моб случайно вышел из области атаки, удаляем из пула
        }

        private void OnTriggerStay(Collider other)
        {
            
            /*
            if (!ViewModel.FsmWarrior.IsGoToMob()) return;
            if (!other.gameObject.CompareTag("MobVisible")) return;
            var mobBinder = other.gameObject.GetComponent<MobVisibleBinder>();
            if (mobBinder.ViewModel.UniqueId == ViewModel.MobTarget.CurrentValue.UniqueId)
            {
                ViewModel.FsmWarrior.Fsm.SetState<FsmWarriorAttack>(mobBinder.ViewModel);
            }
            */

        }
    }
}