using System;
using System.Collections;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace Game.GamePlay.View.Towers
{
    /**
     * Основой Binder башни, связывает все действия
     */
    public abstract class TowerBaseBinder<T> : MonoBehaviour, ITowerBaseBinder where T : TowerViewModel
    {
        [SerializeField] protected Transform container;
        [SerializeField] protected VisualEffect after;
        [SerializeField] protected VisualEffect before;
        [SerializeField] protected AreaBinder areaBinder;
        
        protected Coroutine MainCoroutine;
        protected T ViewModel;
        protected TowerBinder TowerBinder;

        private Sequence Sequence { get; set; }
        private ReactiveProperty<Vector3> _firsTarget;
        protected DisposableBag _disposables;

        private void OnEnable()
        {
            after.gameObject.SetActive(true);
            before.gameObject.SetActive(true);
            after.Stop();
            before.Stop();
        }

        public void Bind(TowerViewModel viewModel)
        {
            // ПОДПИСКИ //
            ViewModel = (T)viewModel;
            viewModel.Position.Subscribe(p =>
            {
                transform.position = new Vector3(
                    p.x,
                    transform.position.y,
                    p.y
                );
            }).AddTo(ref _disposables);
            
            CreateTower();
            CreateArea();
            
            //Если есть площадь, то подписываемся на события
            if (areaBinder != null)
            {
                ViewModel.ShowArea.Subscribe(show =>
                {
                    if (show)
                    {
                        areaBinder.Show(ViewModel.GetAreaRadius());
                    }
                    else
                    {
                        areaBinder.Hide();
                    }
                }).AddTo(ref _disposables);
            }
            
            //Запуск эффекта обновления уровня
            ViewModel.Level.Skip(1).Subscribe(_ =>
            {
                StartCoroutine(EffectsUpgradeTower());
                RestartAfterUpdate();
            }).AddTo(ref _disposables);
            //Смена модели при обновлении на четных уровнях
            ViewModel.NumberModel.Skip(1).Subscribe(number =>
            {
                //Если Префаб уже был, то запускаем анимацию
                Sequence = DOTween.Sequence();
                Sequence
                    .Append(
                        container
                            .DOScale(Vector3.zero, 0.5f)
                            .From(Vector3.one)
                            .SetEase(Ease.OutCubic).SetUpdate(true))
                    .AppendCallback(() =>
                    {
                        DestroyTower();
                        CreateTower();
                    })
                    .Append(
                        container.transform
                            .DOScale(Vector3.one, 0.5f)
                            .SetEase(Ease.InCubic).SetUpdate(true))
                    .OnComplete(() => { Sequence.Kill(); }).SetUpdate(true);
            }).AddTo(ref _disposables);
            OnBind(ViewModel);
        }
        
        protected virtual void OnBind(T viewModel) { }
        
        private IEnumerator EffectsUpgradeTower()
        {
            before.playRate = 1.5f;
            after.playRate = 1.5f;
            before.Play();
            yield return new WaitForSecondsRealtime(0.5f);
            before.Stop();
            after.Play();
            yield return new WaitForSecondsRealtime(0.5f);
            after.Stop();
            ViewModel.FinishEffectLevelUp.OnNext(true);
        }
        
        private void DestroyTower()
        {
            Destroy(TowerBinder.gameObject);
            TowerBinder = null;
        }

        private void CreateTower()
        {
            var towerNumber = ViewModel.NumberModel;
            var towerType = ViewModel.ConfigId;

            var prefabTowerLevelPath =
                $"Prefabs/Gameplay/Towers/{towerType}/{towerType}-{towerNumber}";

            var towerPrefab = Resources.Load<TowerBinder>(prefabTowerLevelPath);
            TowerBinder = Instantiate(towerPrefab, container.transform);
            TowerBinder.Bind(ViewModel);
        }

        private void CreateArea()
        {
            //Нет области Атаки
            if (areaBinder != null) areaBinder.Bind();
        }
        
        /**
         * Перезапуск атаки после обновления башни
         */
        protected abstract void RestartAfterUpdate();

        protected virtual void OnDestroy()
        {
            //StopCoroutine(MainCoroutine);
            if (Sequence.IsActive())
            {
                Sequence.Kill();
                Sequence = null;
            }
            
            Destroy(TowerBinder.gameObject);
            Destroy(TowerBinder);
            _disposables.Dispose();
        }

        public void DestroyGameObject()
        {
            Destroy(transform.gameObject);
        }
    }
}