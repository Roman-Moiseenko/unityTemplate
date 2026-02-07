using System;
using Game.GamePlay.View.Mobs;
using UnityEngine;

namespace Game.GamePlay.View.Warriors
{
    public class WarriorVisibleBinder : MonoBehaviour
    {
        private WarriorViewModel _viewModel;


        public void Bind(WarriorViewModel viewModel)
        {
            _viewModel = viewModel;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Mob")) return;
            var mobBinder = other.gameObject.GetComponent<MobBinder>();
            if (mobBinder.ViewModel.IsFly != _viewModel.IsFly) return; 
            Debug.Log($"Моб {mobBinder.UniqueId} в области видимости воина {_viewModel.UniqueId}");
           // _viewModel.PullTargets.Add(mobBinder.ViewModel);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Mob")) return;
          /*  if (_viewModel.PullTargets.Count != 0)
            {
                //Когда моб выходит из зоны видимости, удаляем из Пула
                var mobBinder = other.gameObject.GetComponent<MobBinder>();
                if (mobBinder == null) return;
                if (mobBinder.ViewModel.IsDead.CurrentValue) return; //Лаг задержки удаления модели
                _viewModel.PullTargets.Remove(mobBinder.ViewModel);
            }
            */
        }
    }
}