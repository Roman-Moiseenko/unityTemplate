using System;
using System.Collections.Generic;
using DI;
using Game.Common;
using Game.GamePlay.Commands.HeroCommand;
using Game.GamePlay.Fsm;
using Game.GamePlay.Root;
using Game.GamePlay.View.Hero;
using Game.GameRoot.Queries.HeroQueries;
using Game.Settings.Gameplay.Entities.Heroes;
using Game.State.Common;
using Game.State.Gameplay;
using Game.State.Gameplay.Statistics;
using Game.State.Maps.Heroes;
using Game.State.Maps.Shots;
using Game.State.Parameters;
using MVVM.CMD;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class HeroesService : IDisposable
    {
        public HeroViewModel HeroViewModel { get; }
        public Dictionary<ParameterType, ParameterData> HeroParameterMap => _heroEntity.Parameters;
        public readonly Dictionary<ParameterType, float> HeroBoosters;

        private FsmHero _fsmHero;
        private readonly HeroEntity _heroEntity;
        private HeroSettings _heroSettings;
        private DisposableBag _disposables;
        private readonly ICommandProcessor _cmd;
        private readonly GameplayStateProxy _gameplayState;

        public HeroesService(
            ICommandProcessor cmd,
            IQueryProcessor qrc,
            FsmHero fsmHero,
            FsmWave fsmWave,
            GameplayStateProxy gameplayState,
            GameplayEnterParams gameplayEnterParams)
        {
            _heroEntity = gameplayState.Hero;
            _gameplayState = gameplayState;
            _cmd = cmd;
            _fsmHero = fsmHero;
            
            var query = new QueryInfoHero(_heroEntity.ConfigId);
            _heroSettings = qrc.Request<QueryInfoHero, HeroSettings>(query);
            

            //Параметры для героя храним в сущности, т.к. она единственная на Геймплей
            foreach (var (paramType, paramData) in gameplayEnterParams.HeroCard.Parameters)
            {
                _heroEntity.Parameters.Add(paramType, paramData.GetCopy());
            }

            UpdateParams(_heroEntity.GameplayLevel.Value);

            HeroViewModel = new HeroViewModel(_heroEntity, _heroSettings, this, fsmWave, _fsmHero);
            _heroEntity.GameplayLevel
                .Skip(1)
                .Subscribe(UpdateParams)
                .AddTo(ref _disposables);

            //Бустеры героя 
            HeroBoosters = gameplayEnterParams.GameplayBoosters.HeroBust;
        }

        private void UpdateParams(int level)
        {
            var settings = _heroSettings
                .GameplayLevels.Find(l => l.Level == level)
                .Parameters;
            foreach (var paramSetting in settings)
            {
                if (_heroEntity.Parameters.TryGetValue(paramSetting.ParameterType, out var param))
                {
                    param.Value *= (1 + paramSetting.Value / 100f);
                }
            }
        }

        /**
         * Рассчитать выстрел от Defence моба
         */
        public ShotData ShotCalculation(TypeDefence typeDefence)
        {
            var damageBooster = 0f;
            var criticalBooster = 0f;
            var damage = 0f;
            //Получаем бустеры урона (damage, crit)
            if (HeroBoosters.TryGetValue(ParameterType.Damage, out var damageParam)) damageBooster = damageParam;
            if (HeroBoosters.TryGetValue(ParameterType.Critical, out var criticalParam))
                criticalBooster = criticalParam;

            //Расчитываем урон
            if (_heroEntity.Parameters.TryGetValue(ParameterType.Damage, out var parameter)) damage = parameter.Value;
            if (_heroEntity.Parameters.TryGetValue(ParameterType.DamageArea, out parameter)) damage = parameter.Value;

            damage += damage * damageBooster / 100; //Добавляем бустер урона
            var damageType = _heroEntity.IsSingleTarget ? DamageType.Normal : DamageType.MassDamage;

            //Рассчитываем крит
            if (_heroEntity.Parameters.TryGetValue(ParameterType.Critical, out var criticalParameter))
            {
                if (MyFunc.IsChance(criticalParameter.Value + criticalBooster))
                {
                    damageType = DamageType.Critical;
                    damage *= 2.0f;
                }
            }
            if (_heroEntity.Defence.Previous() == typeDefence) damage *= 0.8f;
            if (_heroEntity.Defence.Next() == typeDefence) damage *= 1.2f;
            
            var shotData = new ShotData
            {
                ConfigId = _heroEntity.ConfigId,
                Single = _heroEntity.IsSingleTarget,
                Damage = damage,
                //Debuff = debuff,
                DamageType = damageType,
                TypeEntity = TypeEntityStatisticDamage.Skill,
            };
            return shotData;
        }


        public void SetDamageAfterShot(TypeDefence defence, int mobId)
        {
            var shot = ShotCalculation(defence);
            shot.MobEntityId = mobId;
            _gameplayState.Shots.Add(shot);
        }


        public bool LevelUpHero()
        {
            var command = new CommandHeroLevelUp();
            return _cmd.Process(command);
        }

        public bool SetPlacement(Vector2Int position)
        {
            //Новая точка размещения героя
            var command = new CommandPlaceHero(position);
            return _cmd.Process(command);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}