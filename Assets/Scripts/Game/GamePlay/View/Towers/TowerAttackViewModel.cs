using System;
using System.Collections.Generic;
using Game.GamePlay.Fsm;
using Game.GamePlay.Services;
using Game.GamePlay.View.Mobs;
using Game.State.Maps.Towers;
using Game.State.Root;
using ObservableCollections;
using R3;

namespace Game.GamePlay.View.Towers
{
    public class TowerAttackViewModel : TowerViewModel
    {
        public ObservableDictionary<int, MobViewModel> MobTargets = new();
        public ObservableList<MobViewModel> PullTargets = new();
        public bool IsMultiShot => _towerEntity.IsMultiShot;
        public bool IsSingleTarget => _towerEntity.IsSingleTarget;
        public float Speed = 0f;
        
        public ReactiveProperty<float> MaxDistance = new(0f);
        public float MinDistance = 0f;  
        
        //Кеш подписок на смерть моба
        private readonly Dictionary<int, IDisposable> _mobDisposables = new();        

        public TowerAttackViewModel(TowerEntity towerEntity, GameplayStateProxy gameplayState,
            TowersService towersService, FsmTower fsmTower) : base(towerEntity, gameplayState, towersService, fsmTower)
        {
            if (towerEntity.Parameters.TryGetValue(TowerParameterType.Speed, out var towerSpeed))
                Speed = towerSpeed.Value;
            
            if (towerEntity.Parameters.TryGetValue(TowerParameterType.MinDistance, out var towerMinDistance))
                MinDistance = towerMinDistance.Value;
            Level.Subscribe(level =>
            {
                if (towerEntity.Parameters.TryGetValue(TowerParameterType.Distance, out var towerDistance))
                    MaxDistance.Value = towerDistance.Value;
                if (towerEntity.Parameters.TryGetValue(TowerParameterType.MaxDistance, out var towerMaxDistance))
                    MaxDistance.Value = towerMaxDistance.Value;
            });
            //Для TowerAttack
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
                if (_mobDisposables.TryGetValue(target.UniqueId, out var disposable)) disposable.Dispose();
                _mobDisposables.Remove(target.UniqueId);
                MobTargets.Remove(target.UniqueId);
            });

            //При удалении из цели, попытка добавить из пулла
            MobTargets.ObserveRemove().Subscribe(e =>
            {
                //При мультишоте цель автоматически добавляется при попадании в Пулл
                if (!IsMultiShot && PullTargets.Count > 0) SetTarget(PullTargets[0]); //Первый из списка
            });
        }

        private void SetTarget(MobViewModel viewModel)
        {
            if (!IsMultiShot && MobTargets.Count != 0) return;

            if (MobTargets.TryGetValue(viewModel.UniqueId, out var value)) return;
            MobTargets.TryAdd(viewModel.UniqueId, viewModel);
        }

        private void RemoveTarget(MobViewModel mobBinderViewModel)
        {
            MobTargets.Remove(mobBinderViewModel.UniqueId);
        }


        /**
         * Башня наносящая урон
         */
        public void SetDamageAfterShot(MobViewModel mobViewModel)
        {
            var shot = _towerEntity.GetShotParameters(mobViewModel.Defence);
            shot.MobEntityId = mobViewModel.UniqueId;
            _gameplayState.Shots.Add(shot);
        }

        /**
         * Проверяем на совместимость Башни и моба для нанесения урона
         */
        public bool IsTargetForDamage(bool mobIsFly)
        {
            switch (_towerEntity.TypeEnemy)
            {
                case TowerTypeEnemy.Universal:
                case TowerTypeEnemy.Air when mobIsFly:
                case TowerTypeEnemy.Ground when !mobIsFly:
                    return true;
                default:
                    return false;
            }
        }
    }
}