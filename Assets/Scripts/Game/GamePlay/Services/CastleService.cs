using System;
using System.Collections;
using DI;
using Game.Common;
using Game.GamePlay.Fsm;
using Game.GamePlay.Root;
using Game.GamePlay.View.Castle;
using Game.State.Gameplay;
using Game.State.Maps.Castle;
using Game.State.Research;
using MVVM.CMD;
using ObservableCollections;
using R3;
using Scripts.Utils;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class CastleService : IDisposable
    {
        public readonly ObservableList<float> RepairBuffer = new();
        private readonly ReactiveProperty<float> CurrenHealth;
        public CastleViewModel CastleViewModel { get; }

        private readonly ICommandProcessor _cmd;
        private readonly CastleEntity _castleEntity;
        private Coroutines _coroutines;

        private readonly GameplayBoosters _gameplayBoosters;
        private DisposableBag _disposables = new();
        private Coroutine _repairCoroutine; // Ссылка на активную корутину восстановления
        private bool _isDisposed; // Флаг, что сервис уже уничтожен

        /**
           * При загрузке создаем все view-модели из реактивного списка всех строений
           * Подписываемся на событие добавления в массив Proxy сущностей
          */
        public CastleService(
            DIContainer container,
            GameplayStateProxy gameplayState,
            GameplayEnterParams gameplayEnterParams
        )
        {
            _coroutines = GameObject.Find(AppConstants.COROUTINES).GetComponent<Coroutines>();
            _castleEntity = gameplayState.Castle;
            CurrenHealth = _castleEntity.CurrenHealth;
            CastleViewModel = new CastleViewModel(_castleEntity, gameplayState);
            //TODO Увеличить урон, скорость и HP от CastleResearch
            _gameplayBoosters = gameplayEnterParams.GameplayBoosters;

            _castleEntity.CurrenHealth.Subscribe(h =>
            {
                if (h < _castleEntity.FullHealth && !_castleEntity.IsReduceHealth.Value)
                {
                    //Первый запуск корутина восстановления
                    _castleEntity.IsReduceHealth.Value = true;
                    _repairCoroutine = _coroutines.StartCoroutine(RepairHealth());
                }
            }).AddTo(ref _disposables);

            _castleEntity.IsDead
                .Where(v => v)
                .Subscribe(v =>
                {
                    //TODO Перенести в GameplayService
                }).AddTo(ref _disposables);
        }

        /**
         * Корутин восстановления здоровья
         */
        private IEnumerator RepairHealth()
        {
            RepairBuffer.Add(_castleEntity.ReduceHealth); //Буфер для отображения в UI
            _castleEntity.Repair();

            yield return new WaitForSeconds(AppConstants.SPEED_REDICE_CASTLE);

            if (_castleEntity.CurrenHealth.Value < _castleEntity.FullHealth)
            {
                //Перезапуск, пока не восстановили
                if (!_isDisposed)
                    _repairCoroutine = _coroutines.StartCoroutine(RepairHealth());
            }
            else
            {
                _castleEntity.IsReduceHealth.Value = false;
                _repairCoroutine = null;
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            
            if (_repairCoroutine != null)
            {
                // Проверяем, не уничтожен ли уже объект корутин
                if (_coroutines != null)
                {
                    _coroutines.StopCoroutine(_repairCoroutine);
                }
                _repairCoroutine = null;
            }
            CastleViewModel.Dispose();
            _disposables.Dispose();
            _coroutines = null;
            // НЕ дизпоузим CurrenHealth — это ссылка на castleEntity.CurrenHealth (state),
            // его дизпоузит CastleEntity.Dispose() в GameplayStateProxy.
            // Двойной Dispose приведёт к ObjectDisposedException при повторном входе.
            // CurrenHealth?.Dispose();
        }
    }
}
