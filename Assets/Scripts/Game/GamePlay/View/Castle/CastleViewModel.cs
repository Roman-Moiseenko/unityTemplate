using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.GamePlay.Services;
using Game.GamePlay.View.Mobs;
using Game.State.Common;
using Game.State.Gameplay;
using Game.State.Gameplay.Statistics;
using Game.State.Maps.Castle;
using Game.State.Maps.Mobs;
using Game.State.Maps.Shots;
using Game.State.Root;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Castle
{
    public class CastleViewModel : IHasHeathViewModel, IDisposable
    {
        public CastleEntity CastleEntity { get; }
        public int UniqueId => CastleEntity.UniqueId;
        public readonly string ConfigId;
        public Vector2Int Position { get; }
        public ReactiveProperty<MobViewModel> MobTarget = new();

        public ObservableList<MobViewModel> PullTargets = new();

        //Кеш подписок на смерть моба
        private readonly Dictionary<int, IDisposable> _mobDisposables = new();
        public float Speed => CastleEntity.Speed;
        private DisposableBag _disposables = new();
        private readonly CastleService _castleService;

        public ReadOnlyReactiveProperty<bool> IsDead => CastleEntity.IsDead;

        public CastleViewModel(CastleEntity castleEntity,
            CastleService castleService)
        {
            _castleService = castleService;
            ConfigId = castleEntity.ConfigId;
            CastleEntity = castleEntity;
            Position = castleEntity.Position;

            //** Логика ведения целей **//
            PullTargets.ObserveAdd().Subscribe(e =>
            {
                //Моб попал в пулл
                var target = e.Value;
                //При его смерти - удаляем из пула
                var disposable = target.IsDead.Where(x => x).Subscribe(_ => { PullTargets.Remove(target); });
                _mobDisposables.Add(target.UniqueId, disposable); //Кеш подписок на смерть моба
                SetTarget(target); //Добавляем его цель (если мультишот, то добавляется, для одиночного идет проверка)
            }).AddTo(ref _disposables);

            //При удалении из пула (убит или вышел с дистанции) - удалить из цели
            PullTargets.ObserveRemove().Subscribe(e =>
            {
                var target = e.Value;
                if (target == null) return;

                // Если удаленный моб был текущей целью - сбрасываем
                if (MobTarget.CurrentValue != null &&
                    MobTarget.CurrentValue.UniqueId == target.UniqueId)
                {
                    MobTarget.OnNext(null);
                }

                // Очищаем подписку на смерть
                if (_mobDisposables.TryGetValue(target.UniqueId, out var disposable))
                {
                    disposable.Dispose();
                    _mobDisposables.Remove(target.UniqueId);
                }
            }).AddTo(ref _disposables);

            // Когда цель стала null - ищем новую живую цель
            MobTarget.Where(x => x == null).Subscribe(_ =>
            {
                // Ищем первого живого моба в пулле
                var aliveTarget = PullTargets.FirstOrDefault(m => m != null && !m.IsDead.CurrentValue);
                if (aliveTarget != null)
                {
                    SetTarget(aliveTarget);
                }
            }).AddTo(ref _disposables);
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
            if (mobViewModel == null) return;
            if (mobViewModel.IsDead.CurrentValue) return; // Не берем в цель мертвого
            if (MobTarget.CurrentValue == null) MobTarget.OnNext(mobViewModel);
        }

        private void RemoveTarget(MobViewModel mobViewModel)
        {
            if (mobViewModel == null)
            {
                MobTarget.OnNext(null);
                return;
            }

            if (MobTarget.CurrentValue != null &&
                MobTarget.CurrentValue.UniqueId == mobViewModel.UniqueId)
                MobTarget.OnNext(null);
        }

        //TODO Возможно вынести в сервис и сделать через Command
        public void SetDamageAfterShot()
        {
            //Доп.проверка на случай убийства моба
            if (MobTarget.CurrentValue == null) return;
            _castleService.SetDamageAfterShot(MobTarget.CurrentValue.UniqueId);
        }

        public void DamageReceived(float damage, TypeDefence defence)
        {
            CastleEntity.DamageReceived(damage);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}