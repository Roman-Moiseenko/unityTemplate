using System;
using System.Collections.Generic;
using Game.GamePlay.Services;
using Game.GamePlay.View.Mobs;
using Game.State.Maps.Castle;
using Game.State.Maps.Mobs;
using Game.State.Maps.Shots;
using Game.State.Root;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Castle
{
    public class CastleViewModel: IHasHeathViewModel
    {
        private readonly GameplayStateProxy _gameplayState;
        public CastleEntity CastleEntity { get; }
        public int UniqueId => CastleEntity.UniqueId;
        public ReadOnlyReactiveProperty<int> Level { get; }
        public readonly string ConfigId;
        public Vector2Int Position { get; }
        public ReactiveProperty<MobViewModel> MobTarget = new();
        public ObservableList<MobViewModel> PullTargets = new();
        //Кеш подписок на смерть моба
        private readonly Dictionary<int, IDisposable> _mobDisposables = new();
        public float Speed => CastleEntity.Speed;

        public ReadOnlyReactiveProperty<bool> IsDead => CastleEntity.IsDead;
        
        public CastleViewModel(CastleEntity castleEntity,
            GameplayStateProxy gameplayState)
        {
            _gameplayState = gameplayState;
            ConfigId = castleEntity.ConfigId;
            CastleEntity = castleEntity;
            Position = castleEntity.Position;
            
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
                if (PullTargets.Count == 0)
                {
                    MobTarget.OnNext(null);
                }
            });
            MobTarget.Where(x => x == null).Subscribe(_ =>
            {
                if (PullTargets.Count > 0) SetTarget(PullTargets[0]);
            });
        }

        public bool IsPosition(Vector2 position)
        {
            var delta = 0.5f; //Половина ширины клетки
            var _x0 = Position.x;
            var _y0 = Position.y;
            if ((position.x < _x0 + delta && position.x > _x0 - delta) && 
                (position.y < _y0 + delta && position.y > _y0 - delta))
                return true;
            return false;
        }

        private void SetTarget(MobViewModel mobViewModel)
        {
            if (MobTarget.CurrentValue == null) MobTarget.OnNext(mobViewModel);
        }

        private void RemoveTarget(MobViewModel mobViewModel)
        {
            if (MobTarget.CurrentValue.UniqueId == mobViewModel.UniqueId) MobTarget.OnNext(null);
        }

        public void SetDamageAfterShot()
        {
            //Доп.проверка на случай убийства моба
            if (MobTarget.CurrentValue == null) return;
            var shot = new ShotData
            {
                Damage = CastleEntity.Damage,
                DamageType = DamageType.Normal, //TODO Возможно сделать крит-шанс
                Position = MobTarget.CurrentValue.PositionTarget.CurrentValue,
                Single = true,
                MobEntityId = MobTarget.CurrentValue.UniqueId,
            };
            _gameplayState.Shots.Add(shot);
        }

        public void DamageReceived(float damage, MobDefence defence)
        {
            Debug.Log("Урон по замку " + damage);
            CastleEntity.DamageReceived(damage);
        }
    }
}