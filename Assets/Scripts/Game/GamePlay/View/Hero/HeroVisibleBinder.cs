using Game.GamePlay.View.Castle;
using Game.GamePlay.View.Mobs;
using UnityEngine;

namespace Game.GamePlay.View.Hero
{
    public class HeroVisibleBinder : MonoBehaviour
    {
        private HeroViewModel _viewModel;

        private void Awake()
        {
            Physics.reuseCollisionCallbacks = true;
        }

        public void Bind(HeroViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Mob")) return; //Обрабатываем только мобов
            var mobBinder = other.gameObject.GetComponent<MobBinder>();
            if (mobBinder == null || mobBinder.ViewModel == null) return;
            _viewModel.PullTargets.Add(mobBinder.ViewModel); 
        }
        

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Mob")) return; //Обрабатываем только мобов
            if (_viewModel.PullTargets.Count != 0)
            {
                //Когда моб выходит из зоны видимости, удаляем из Пула
                var mobBinder = other.gameObject.GetComponent<MobBinder>();
                if (mobBinder == null || mobBinder.ViewModel == null) return;
                if (mobBinder.ViewModel.IsDead.CurrentValue) return; //Лаг задержки удаления модели
                
                _viewModel.PullTargets.Remove(mobBinder.ViewModel);
            }
        }
    }
}
