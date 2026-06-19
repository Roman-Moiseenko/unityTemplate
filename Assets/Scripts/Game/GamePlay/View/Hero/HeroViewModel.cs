using System;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.HeroStates;
using Game.GamePlay.Fsm.WaveStates;
using Game.GamePlay.Services;
using Game.GamePlay.View.Mobs;
using Game.Settings.Gameplay.Entities.Heroes;
using Game.State.Common;
using Game.State.Gameplay;
using Game.State.Maps.Heroes;
using Game.State.Parameters;
using R3;
using UnityEngine;
using ObservableCollections;

namespace Game.GamePlay.View.Hero
{
    public class HeroViewModel : IDisposable
    {
        private readonly HeroEntity _heroEntity;
        public string ConfigId => _heroEntity.ConfigId;
        public ReactiveProperty<Vector2> Position => _heroEntity.Position;
        public ReactiveProperty<Vector2Int> Placement => _heroEntity.Placement;
        public ReactiveProperty<int> GameplayLevel => _heroEntity.GameplayLevel;
        
        public ReactiveProperty<MobViewModel> MobTarget = new();

        public ObservableList<MobViewModel> PullTargets = new();
        public TypeEpic EpicLevel => _heroEntity.EpicLevel;
        public Dictionary<ParameterType, ParameterData> Parameters => _heroEntity.Parameters; 

        //Кеш подписок на смерть моба
        private readonly Dictionary<int, IDisposable> _mobDisposables = new();
        private DisposableBag _disposables;
        private readonly HeroesService _heroesService;

        public HeroViewModel(
            HeroEntity heroEntity, 
            HeroSettings heroSettings, 
            HeroesService heroesService, 
            FsmWave fsmWave,
            FsmHero fsmHero)
        {
            //Debug.Log($"HeroViewModel {heroEntity.ConfigId}");
            _heroEntity = heroEntity;
            _heroesService = heroesService;

            //** Логика ведения целей **//
            PullTargets.ObserveAdd().Subscribe(e =>
            {
                var target = e.Value; //Моб попал в пулл
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
                    MobTarget.CurrentValue.UniqueId == target.UniqueId) MobTarget.OnNext(null);
                
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
                if (aliveTarget != null) SetTarget(aliveTarget);
                
            }).AddTo(ref _disposables);
            
            //По окончании волны проверяем, было ли изменение расположения
            fsmWave.Fsm.StateCurrent
                .Where(x => x.GetType() == typeof(FsmStateWaveEnd))
                .Subscribe(_ =>
                {
                    var dx = Math.Abs(Position.CurrentValue.x - Placement.CurrentValue.x);
                    var  dy = Math.Abs(Position.CurrentValue.y - Placement.CurrentValue.y);
                    if (dx > 0.5 || dy > 0.5) fsmHero.Fsm.SetState<FsmHeroMoving>();
                })
                .AddTo(ref _disposables);
        }

        public bool IsPosition(Vector2 position)
        {
            const float delta = 0.5f; //Половина ширины клетки
            var x0 = Position.CurrentValue.x;
            var y0 = Position.CurrentValue.y;
            if ((position.x < x0 + delta && position.x > x0 - delta) &&
                (position.y < y0 + delta && position.y > y0 - delta))
                return true;
            return false;
        }
        
        
        private void SetTarget(MobViewModel mobViewModel)
        {
            if (mobViewModel == null) return;
            if (mobViewModel.IsDead.CurrentValue) return; // Не берем в цель мертвого
            if (MobTarget.CurrentValue == null) MobTarget.OnNext(mobViewModel);
        }

        //MAINDO В Binder по списку из радиуса наносим всем урон, после выстрела ...
        public void SetDamageAfterShot(MobViewModel mobViewModel)
        {
            //Доп.проверка на случай убийства моба
            if (MobTarget.CurrentValue == null) return;
            _heroesService.SetDamageAfterShot(mobViewModel.Defence, mobViewModel.UniqueId);
        }
        public void Dispose()
        {
            MobTarget?.Dispose();
            PullTargets.Clear();
            _disposables.Dispose();
        }
    }
}