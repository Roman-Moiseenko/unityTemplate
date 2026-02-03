using System;
using System.Collections;
using System.Collections.Generic;
using Game.GamePlay.View.Mobs;
using Game.State.Maps.Shots;
using Game.State.Maps.Warriors;
using Game.State.Root;
using ObservableCollections;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Game.GamePlay.View.Warriors
{
    public class WarriorViewModel
    {
        private readonly WarriorEntity _warriorEntity;
        public int UniqueId => _warriorEntity.UniqueId;
        public int ParentId => _warriorEntity.ParentId;
        public string ConfigId => _warriorEntity.ConfigId;
        public float Speed => _warriorEntity.Speed;
        public ReadOnlyReactiveProperty<bool> IsDead => _warriorEntity.IsDead;

        public Vector3 StartPosition;
        public Vector3 PlacementPosition;
        public ReactiveProperty<MobViewModel> MobTarget = new();
        public ObservableList<MobViewModel> PullTargets = new();
        private readonly Dictionary<int, IDisposable> _mobDisposables = new();
        
        private readonly GameplayStateProxy _gameplayState;
        public float MaxHealth;
        public ReactiveProperty<float> CurrentHealth;

        public WarriorViewModel(WarriorEntity warriorEntity, GameplayStateProxy gameplayState)
        {
            _warriorEntity = warriorEntity;
            _gameplayState = gameplayState;

            MaxHealth = _warriorEntity.Health.CurrentValue;
            CurrentHealth = _warriorEntity.Health;

            StartPosition = new Vector3(
                warriorEntity.StartPosition.x, 
                warriorEntity.IsFly ? 1 : 0, 
                warriorEntity.StartPosition.y);
            PlacementPosition = new Vector3(
                warriorEntity.PlacementPosition.x + Mathf.Clamp(Random.insideUnitSphere.x, -0.3f, 0.3f),
                warriorEntity.IsFly ? 1 : 0, 
                warriorEntity.PlacementPosition.y + Mathf.Clamp(Random.insideUnitSphere.y, -0.3f, 0.3f));
            
            
            //** Логика ведения целей **//
            PullTargets.ObserveAdd().Subscribe(e =>
            {
                //Моб попал в пулл
                var target = e.Value;
                //При его смерти - удаляем из пула
                var disposable = target.IsDead.Where(x => x).Subscribe(_ => PullTargets.Remove(target));
                _mobDisposables.Add(target.UniqueId, disposable); //Кеш подписок на смерть моба
                SetTarget(target); //Добавляем его цель (если мультишот, то добавляется, для одиночного идет проверка)
            });            
            //При удалении из пула (убит или вышел с дистанции) - удалить из цели
            PullTargets.ObserveRemove().Subscribe(e =>
            {
                var target = e.Value;
                _mobDisposables.Remove(target.UniqueId);
                RemoveTarget(target);
            });
            MobTarget.Where(x => x == null).Subscribe(_ =>
            {
                if (PullTargets.Count > 0) SetTarget(PullTargets[0]);
            });
            
        }

        private void SetTarget(MobViewModel mobViewModel)
        {
            //Проверка на совпадения типа мобов
            if (mobViewModel.IsFly != _warriorEntity.IsFly) return;
            
            if (MobTarget.CurrentValue == null) MobTarget.OnNext(mobViewModel);
        }

        private void RemoveTarget(MobViewModel mobViewModel)
        {
            if (MobTarget.CurrentValue == mobViewModel) MobTarget.OnNext(null);
        }

        public void ClearTarget()
        {
            MobTarget.OnNext(null);
        }

        public void SetDamageAfterShot()
        {
            //Доп.проверка на случай убийства моба
            if (MobTarget.CurrentValue == null) return;
            var shot = new ShotData
            {
                Damage = _warriorEntity.Damage,
                DamageType = DamageType.Normal,
                Position = MobTarget.CurrentValue.PositionTarget.CurrentValue,
                Single = true,
                MobEntityId = MobTarget.CurrentValue.UniqueId,
            };
//            Debug.Log("Урон от Warrior " + UniqueId + " Мобу " + MobTarget.CurrentValue.UniqueId);
            _gameplayState.Shots.Add(shot);
        }
        
    }
}