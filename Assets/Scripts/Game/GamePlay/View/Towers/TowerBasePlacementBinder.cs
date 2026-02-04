using System.Collections;
using Game.GamePlay.View.Warriors;
using MVVM.Storage;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerBasePlacementBinder : TowerBaseBinder<TowerPlacementViewModel>
    {
        private ObservableList<WarriorBinder> warriors = new();
        
        protected override void OnBind(TowerPlacementViewModel viewModel)
        {
           // ViewModel.AddWarriorsTower();

            Debug.Log(viewModel.Warriors.Count);
            foreach (var warriorViewModel in viewModel.Warriors)
            {
                CreateWarrior(warriorViewModel);

            }
            viewModel.Warriors.ObserveAdd().Subscribe(e =>
            {
                var warriorViewModel = e.Value;
                CreateWarrior(warriorViewModel);
            });
            
            MainCoroutine = StartCoroutine(PlacementUpdateTower());
        }
        
        protected override void RestartAfterUpdate()
        {
            //TODO Обновляем воинов
            ViewModel.UpdateAndRestartWarriors();
        }
        
        private IEnumerator PlacementUpdateTower()
        {
            while (true)
            {
                if (ViewModel.IsDeadAllWarriors())
                {
                    //TODO Ускорение при быстром вызове волны
                    yield return new WaitForSeconds(10f);
                    ViewModel.AddWarriorsTower();
                }
                yield return null;
            }
        }
        
        
        private void CreateWarrior(WarriorViewModel warriorViewModel)
        {
            var prefabWarriorPath =
                $"Prefabs/Gameplay/Warriors/Warrior-{warriorViewModel.ConfigId}"; //Перенести в настройки уровня
            var warriorPrefab = Resources.Load<WarriorBinder>(prefabWarriorPath);
            var createdWarrior = Instantiate(warriorPrefab, transform);
            createdWarrior.Bind(warriorViewModel);
            warriors.Add(createdWarrior);
        }
    }
}