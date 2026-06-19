using System;
using System.Collections.Generic;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.HeroStates;
using Game.GamePlay.Fsm.TowerStates;
using Game.GamePlay.Root;
using Game.GamePlay.Services;
using Game.GamePlay.View.Hero;
using Game.GamePlay.View.Towers;
using Game.Settings;
using Game.Settings.Gameplay.Entities.Heroes;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Common;
using Game.State.Parameters;
using Game.State.Research;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.UI.PanelGateWave.InfoHero
{
    public class InfoHeroViewModel : IDisposable
    {
        public readonly ReactiveProperty<bool> ShowInfoHero = new(false);
        public readonly ReactiveProperty<bool> UpdateInfoBackgroundTower = new(false);
        public readonly ReactiveProperty<Vector3> PositionInfoHero = new(Vector3.zero);
        private readonly GameplayCamera _cameraService;
        
        public readonly Dictionary<ParameterType, Vector2> BaseParameters = new(); 
        public readonly Dictionary<ParameterType, float> BoosterParameters = new();
        
        private Vector2Int _towerPrevious = Vector2Int.zero;
       // public TowerViewModel TowerViewModel;

        public string NameTower;
        public TypeEpic EpicLevel;
        public int Level;
        public TypeDefence Defence;
        
        private readonly GameSettings _gameSettings;
        private readonly List<HeroSettings> _settingsHeroes;
        private readonly GameplayBoosters _gameplayBooster;
        private readonly Dictionary<ParameterType, float> _heroBoosters;
        private readonly HeroViewModel _heroViewModel;

        private DisposableBag _disposables;
        public InfoHeroViewModel(DIContainer container)
        {
            var positionCamera = container.Resolve<Subject<Unit>>(AppConstants.CAMERA_MOVING);
            var fsmHero = container.Resolve<FsmHero>();
            var gameSettings = container.Resolve<ISettingsProvider>().GameSettings;
            var heroesService = container.Resolve<HeroesService>();
            _settingsHeroes = gameSettings.HeroesSettings.AllHeroes;
            
            _gameplayBooster = container.Resolve<GameplayEnterParams>().GameplayBoosters;
            _heroBoosters = heroesService.HeroBoosters;
            _heroViewModel = heroesService.HeroViewModel;
            
            fsmHero.Fsm.StateCurrent.Subscribe(state =>
            {
                if (state.GetType() == typeof(FsmHeroUnSelected) || 
                    state.GetType() == typeof(FsmHeroPlacement))
                {
                    ShowInfoHero.Value = false;
                    _towerPrevious = Vector2Int.zero;
                }
                if (state.GetType() == typeof(FsmHeroSelected))
                {
                    PrepareAndShowInfo();
                }
                //TODO Сделать для других состояний
            }).AddTo(ref _disposables);
            
            _cameraService = container.Resolve<GameplayCamera>();
 
            //Изменилась позиция камеры
            positionCamera.Subscribe(n =>
            {
                if (ShowInfoHero.CurrentValue) NewPositionTowerInfo();
            }).AddTo(ref _disposables);
        }
        
        private void NewPositionTowerInfo()
        {
            var p = new Vector3(_towerPrevious.x - 0.5f, 1f, _towerPrevious.y - 0.5f);
            var v= _cameraService.Camera.WorldToScreenPoint(p);
            //v.z = 0;
            //v.y += 100;
            PositionInfoHero.Value = v;
        }

        private void PrepareAndShowInfo()
        {
            
            
            if (_towerPrevious == _heroViewModel.Position.CurrentValue)
            {
                if (ShowInfoHero.CurrentValue) ShowInfoHero.OnNext(false);
                _towerPrevious = Vector2Int.zero;
            }
            else
            {
                //TowerViewModel = towerViewModel;

                var config = _settingsHeroes.Find(t => t.ConfigId == _heroViewModel.ConfigId);
                NameTower = config.TitleLid;
                Defence = config.Defence;
                EpicLevel = _heroViewModel.EpicLevel;
                Level = _heroViewModel.GameplayLevel.CurrentValue;


                _towerPrevious = Vector2Int.RoundToInt(_heroViewModel.Position.CurrentValue);
                BaseParameters.Clear();
                BoosterParameters.Clear();
                
                foreach (var (parameterType, value) in _heroBoosters)
                {
                    //TODO Для всех Damage сделать проверка
                    if (parameterType == ParameterType.Damage)
                    {
                        if (_heroViewModel.Parameters.TryGetValue(ParameterType.Damage, out _))
                            BoosterParameters.Add(ParameterType.Damage, value);
                        if (_heroViewModel.Parameters.TryGetValue(ParameterType.DamageArea, out _))
                            BoosterParameters.Add(ParameterType.DamageArea, value);
                    }
                    else
                    {
                        BoosterParameters.Add(parameterType, value);    
                    }
                    
                }
                
                
                //Урон, все 3
                if (_heroViewModel.Parameters.TryGetValue(ParameterType.Damage, out var damage))
                {
                    var paramVector = new Vector2(damage.Value, 0);
                    //проверка на бустер
                    if (BoosterParameters.TryGetValue(ParameterType.Damage, out var damageBooster)) paramVector.y = damageBooster;
                    BaseParameters.Add(ParameterType.Damage, paramVector);
                }
                if (_heroViewModel.Parameters.TryGetValue(ParameterType.DamageArea, out var damageArea))
                {
                    var paramVector = new Vector2(damageArea.Value, 0);
                    //проверка на бустер
                    if (BoosterParameters.TryGetValue(ParameterType.DamageArea, out var damageBooster)) paramVector.y = damageBooster;
                    BaseParameters.Add(ParameterType.DamageArea, paramVector);
                }
                //TODO Добавить Высокий урон, низкий урон
                
                //Частота
                if (_heroViewModel.Parameters.TryGetValue(ParameterType.Speed, out var speed))
                {
                    var paramVector = new Vector2(speed.Value, 0);
                    //проверка на бустер
                    if (BoosterParameters.TryGetValue(ParameterType.Speed, out var speedBooster)) paramVector.y = speedBooster;
                    BaseParameters.Add(ParameterType.Speed, paramVector);
                }
                
                //Крит, Замедление, Оглушение, Здоровье, Перезарядка  если кол-во иконок меньше 3х
                if (BaseParameters.Count < 3 
                    && _heroViewModel.Parameters.TryGetValue(ParameterType.Critical, out var critical))
                {
                    var paramVector = new Vector2(critical.Value, 0);
                    //проверка на бустер
                    if (BoosterParameters.TryGetValue(ParameterType.Critical, out var criticalBooster)) paramVector.y = criticalBooster;
                    BaseParameters.Add(ParameterType.Critical, paramVector);
                }
                
                if (BaseParameters.Count < 3 
                    && _heroViewModel.Parameters.TryGetValue(ParameterType.SlowingDown, out var slow))
                    BaseParameters.Add(ParameterType.SlowingDown, new Vector2(slow.Value, 0));
                if (BaseParameters.Count < 3 
                    && _heroViewModel.Parameters.TryGetValue(ParameterType.Health, out var health))
                    BaseParameters.Add(ParameterType.Health, new Vector2(health.Value, 0));
                
                //TODO Добавить Оглушение, Перезарядка


                
                UpdateInfoBackgroundTower.OnNext(true);
                NewPositionTowerInfo();
                ShowInfoHero.OnNext(true);
            }
        }


        public void Dispose()
        {
            ShowInfoHero?.Dispose();
            UpdateInfoBackgroundTower?.Dispose();
            PositionInfoHero?.Dispose();
            _disposables.Dispose();
        }
    }
}