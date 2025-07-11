using System.Collections;
using System.Collections.Generic;
using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.View.Shots;
using Game.GamePlay.View.Towers;
using Game.State.Maps.Mobs;
using Game.State.Maps.Shots;
using Game.State.Maps.Towers;
using Game.State.Root;
using ObservableCollections;
using R3;
using Scripts.Utils;
using UnityEngine;


namespace Game.GamePlay.Services
{
    public class ShotService
    {
        private readonly GameplayStateProxy _gameplayState;
        private readonly FsmGameplay _fsmGameplay;
        private readonly Coroutines _coroutines;
        private int _uniqueShotId = 0;
        
        public ObservableList<ShotEntity> Shots = new();
        public ObservableList<ShotViewModel> AllShots = new();
        private Dictionary<int, ShotViewModel> _shotsMap = new();
        
        public readonly ReactiveProperty<int> GameSpeed = new(1);
        //TODO Нужны ли данные в конструктор? Возможно настройки базовые?
        public ShotService(GameplayStateProxy gameplayState, FsmGameplay fsmGameplay)
        {
            _gameplayState = gameplayState;
            _fsmGameplay = fsmGameplay;
            GameSpeed = gameplayState.GameSpeed;
            _coroutines = GameObject.Find("[COROUTINES]").GetComponent<Coroutines>();

            AllShots.ObserveAdd().Subscribe(newValue =>
            {
                _coroutines.StartCoroutine(MovingShot(newValue.Value)); 
            });
            AllShots.ObserveRemove().Subscribe(value =>
            {
                _coroutines.StopCoroutine(MovingShot(value.Value));
            });
        }


        public void CreateShot(TowerEntity towerEntity, MobEntity mobEntity)
        {
            var startPosition =
                new Vector3(towerEntity.Position.CurrentValue.x, 1f, towerEntity.Position.CurrentValue.y);
            var shotEntityData = new ShotEntityData
            {
                TowerEntityId = towerEntity.UniqueId,
                MobEntityId = mobEntity.UniqueId,
                ConfigId = towerEntity.ConfigId,
                StartPosition = startPosition,
                Position = startPosition,
                UniqueId = GetUniqueId(),
                Speed = 1, //TODO сделать из настроек 
                Damage = 8, //TODO Из  башни взять + добавить тип урона
            };

            var shotEntity = new ShotEntity(shotEntityData, mobEntity.PositionTarget);
            Shots.Add(shotEntity); //Удалить
            var shotViewModel = new ShotViewModel(shotEntity, this);
            _shotsMap.Add(shotEntity.UniqueId, shotViewModel);
            AllShots.Add(shotViewModel);
        }

        private void RemoteShot(ShotViewModel shotViewModel)
        {
            if (_shotsMap.TryGetValue(shotViewModel.ShotEntityId, out var viewModel))
            {
                Shots.Remove(viewModel._shotEntity); //удаляем сущность
                AllShots.Remove(viewModel); //удаляем модель
                _shotsMap.Remove(shotViewModel.ShotEntityId);
            }
        }

        private int GetUniqueId()
        {
            return _uniqueShotId++;
        }

        public IEnumerator MovingShot(ShotViewModel shotViewModel)
        {
            //Debug.Log("MovingShot Start");
            yield return new WaitUntil(() => !_fsmGameplay.IsGamePause.Value);
            //Debug.Log(shotViewModel.StartPosition + " => " + shotViewModel._shotEntity.FinishPosition.CurrentValue);
            yield return shotViewModel.MovingModel();
            RemoteShot(shotViewModel);
        }
        
        
    }
}