using System;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerBaseBinder : MonoBehaviour
    {
        [SerializeField] private Transform container;
        
        private IDisposable _disposable;
        private TowerViewModel _viewModel;

        private TowerBinder _towerBinder;
        public void Bind(TowerViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            
            _viewModel.NumberModel.Subscribe(number =>
            {
                //Если Префаб уже был, то запускаем анимацию
                if (_towerBinder != null)
                {
                    //После анимации пересоздаем модель с параметром анимации
                    _towerBinder.RemoveTowerAnimation().Where(x => x).Subscribe(_ =>
                    {
                        DestroyTower();
                        _viewModel.IsUpdate = true;
                        CreateTower();
                    });
                }
                else
                {
                    CreateTower();
                } 
            }).AddTo(ref d);
            _disposable = d.Build();
        }

        private void DestroyTower()
        {
            Destroy(_towerBinder.gameObject);
            _towerBinder = null;
        }


        private void CreateTower()
        {
            var towerNumber = _viewModel.NumberModel;
            var towerType = _viewModel.ConfigId;

            var prefabTowerLevelPath =
                $"Prefabs/Gameplay/Towers/{towerType}/{towerType}-{towerNumber}"; //Перенести в настройки уровня
            //var prefabTowerLevelPath = $"Prefabs/Gameplay/Towers/{towerType}/{towerViewModel.GetNameModel()}";

            var towerPrefab = Resources.Load<TowerBinder>(prefabTowerLevelPath);
            _towerBinder = Instantiate(towerPrefab, container.transform);
            _towerBinder.Bind(_viewModel);
            
        }
    }
}