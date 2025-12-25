using System;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerBaseBinder : MonoBehaviour
    {
        [SerializeField] private Transform container;
        [SerializeField] private Animator animator;

        [SerializeField] private ParticleSystem finish;
        [SerializeField] private ParticleSystem start;
        
        
        private IDisposable _disposable;
        private TowerViewModel _viewModel;

        private TowerBinder _towerBinder;
        public void Bind(TowerViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            
            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x,
                transform.position.y,
                viewModel.Position.CurrentValue.y
            );

            _viewModel.Level.Skip(1).Subscribe(_ =>
            {
                //TODO Запускаем анимацию шейдеров и частиц
                start.Play();
                finish.Play();
            });
            
            _viewModel.NumberModel.Subscribe(number =>
            {
                //Если Префаб уже был, то запускаем анимацию
                if (_towerBinder != null)
                {
                    animator.Play("tower_level_up");
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
                $"Prefabs/Gameplay/Towers/{towerType}/{towerType}-{towerNumber}"; 

            var towerPrefab = Resources.Load<TowerBinder>(prefabTowerLevelPath);
            _towerBinder = Instantiate(towerPrefab, container.transform);
            _towerBinder.Bind(_viewModel);
        }
        
        private void OnDestroy()
        {
            _disposable?.Dispose();
        }

        public void AnimationChangeModel()
        {
            DestroyTower();
            CreateTower();
        }

        public void AnimationFinish()
        {
            Debug.Log("Конец анимации");
        }
    }
}