using System.Collections;
using DI;
using Game.Common;
using Game.GamePlay.Fsm;
using Game.GamePlay.View.Castle;
using Game.State.Maps.Castle;
using Game.State.Root;
using MVVM.CMD;
using ObservableCollections;
using R3;
using Scripts.Utils;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class CastleService
    {
        public ObservableList<float> RepairBuffer = new();
        public ReactiveProperty<float> CurrenHealth;
        public CastleViewModel CastleViewModel { get; }

        private readonly ICommandProcessor _cmd;
        private readonly CastleEntity _castleEntity;
        private readonly Coroutines _coroutines;
        private readonly FsmGameplay _fsmGameplay;

        /**
           * При загрузке создаем все view-модели из реактивного списка всех строений
           * Подписываемся на событие добавления в массив Proxy сущностей
          */
        public CastleService(
            DIContainer container,
            CastleEntity castleEntity,
            GameplayStateProxy gameplayState
        )
        {
            _coroutines = GameObject.Find("[COROUTINES]").GetComponent<Coroutines>();
            _castleEntity = gameplayState.Castle;
            _fsmGameplay = container.Resolve<FsmGameplay>();
            //GameSpeed = gameplayState.GameSpeed;
            CurrenHealth = castleEntity.CurrenHealth;
            CastleViewModel = new CastleViewModel(castleEntity);

            _castleEntity.CurrenHealth.Subscribe(h =>
            {
                if (h < _castleEntity.FullHealth && !_castleEntity.IsReduceHealth.Value)
                {
                    //Первый запуск корутина восстановления 
                    _castleEntity.IsReduceHealth.Value = true;
                    _coroutines.StartCoroutine(RepairHealth());
                }
            });

            _castleEntity.IsDead.Subscribe(v =>
            {
                if (v)
                {
                    //TODO Перенести в GameplayService
                }
            });
        }

        /**
         * Корутин восстановления здоровья
         */
        private IEnumerator RepairHealth()
        {
            while (_fsmGameplay.IsPause()) yield return null;

            RepairBuffer.Add(_castleEntity.ReduceHealth); //Буфер для отображения в UI
            _castleEntity.Repair();

            yield return new WaitForSeconds(AppConstants.SPEED_REDICE_CASTLE);

            if (_castleEntity.CurrenHealth.Value < _castleEntity.FullHealth)
            {
                //Перезапуск, пока не восстановили
                _coroutines.StartCoroutine(RepairHealth());
            }
            else
            {
                _castleEntity.IsReduceHealth.Value = false;
            }
        }
    }
}