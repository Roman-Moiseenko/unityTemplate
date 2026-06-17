using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.SkillStates;
using Game.GamePlay.Root;
using Game.GamePlay.View.Skills;
using Game.Settings.Gameplay.Entities;
using Game.Settings.Gameplay.Entities.Skill;
using Game.State.Common;
using Game.State.Gameplay;
using Game.State.Inventory.SkillCards;
using Game.State.Maps.Skills;
using Game.State.Parameters;
using Game.State.Research;
using MVVM.CMD;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class SkillsService : IDisposable
    {
        public readonly Dictionary<string,
            Dictionary<ParameterType, ParameterData>> SkillParametersMap = new();

        public IObservableCollection<SkillViewModel> AllSkills =>
            _allSkills; //Интерфейс менять нельзя, возвращаем через динамический массив

        public readonly ObservableDictionary<string, int> Levels = new(); //Уровни башен по типам ConfigId

        private readonly List<SkillCardData> _baseSkillCards; //
        private readonly ICommandProcessor _cmd;


        private readonly ObservableList<SkillViewModel> _allSkills = new(); //Список примененных навыков
        private readonly List<SkillViewModel> _skillsMap = new(); //Доступные навыки в GamePlay

        //Использовать 
        public SkillViewModel SkillOne;
        public SkillViewModel SkillTwo;

        private readonly Dictionary<string, List<LevelSettings>> _skillSettingsMap = new();
        private readonly GameplayStateProxy _gameplayState;
        private readonly DIContainer _container;
        private readonly GameplayBoosters _gameplayBoosters;

        public Dictionary<string, Dictionary<ParameterType, float>> SkillBoosters = new();
        private readonly FsmSkill _fsmSkill;
        private DisposableBag _disposables = new();

        public SkillsService(
            GameplayStateProxy gameplayState,
            SkillsSettings skillsSettings,
            GameplayEnterParams gameplayEnterParams,
            DIContainer container)
        {
            _container = container;
            _gameplayState = gameplayState;
            _cmd = container.Resolve<ICommandProcessor>();
            _fsmSkill = container.Resolve<FsmSkill>();
            _baseSkillCards = gameplayEnterParams.Skills; //Базовые настройки колоды
            _gameplayBoosters = gameplayEnterParams.GameplayBoosters;

            //Кешируем настройки навыков
            foreach (var skillSettings in skillsSettings.AllSkills)
            {
                _skillSettingsMap[skillSettings.ConfigId] = skillSettings.GameplayLevels;
                Levels[skillSettings.ConfigId] = 1;
            }

            //Кешируем уровень навыков по конфигу
            foreach (var skillEntity in _gameplayState.Skills)
            {
                Levels[skillEntity.ConfigId] = skillEntity.Level.CurrentValue;
            }


            foreach (var skillCardData in _baseSkillCards)
            {
                var param = new Dictionary<ParameterType, ParameterData>();
                foreach (var parameterData in skillCardData.Parameters)
                {
                    param.Add(parameterData.Key, parameterData.Value.GetCopy());
                }

                SkillParametersMap.Add(skillCardData.ConfigId, param);
                for (var i = 1; i <= Levels[skillCardData.ConfigId]; i++)
                {
                    UpdateParams(skillCardData.ConfigId, i); //Увеличиваем параметры по геймплей уровню навыка
                }
            }

            if (_gameplayState.Skills.Count > 0)
            {
                _gameplayState.Skills[0].Parameters = SkillParametersMap[_gameplayState.Skills[0].ConfigId];
                SkillOne = new SkillViewModel(_gameplayState.Skills[0], this, gameplayState);
                _skillsMap.Add(SkillOne);
            }

            if (_gameplayState.Skills.Count > 1)
            {
                _gameplayState.Skills[1].Parameters = SkillParametersMap[_gameplayState.Skills[1].ConfigId];
                SkillTwo = new SkillViewModel(_gameplayState.Skills[1], this,gameplayState);
                _skillsMap.Add(SkillTwo);
            }


            _gameplayState.Skills.ObserveAdd().Subscribe(e =>
            {
                var skillEntity = e.Value;
                skillEntity.Level.Value = Levels[skillEntity.ConfigId]; //Устанавливаем уровень апгрейда
                skillEntity.Parameters = SkillParametersMap[skillEntity.ConfigId];
                if (_gameplayState.Skills.Count > 2) throw new Exception("Ошибка");

                if (SkillOne != null && SkillTwo == null)
                {
                    SkillTwo = new SkillViewModel(skillEntity, this, gameplayState);
                    _skillsMap.Add(SkillTwo);
                }

                if (SkillOne == null)
                {
                    SkillOne = new SkillViewModel(skillEntity, this, gameplayState);
                    _skillsMap.Add(SkillOne);
                }
            }).AddTo(ref _disposables);
            Levels.ObserveChanged().Subscribe(x =>
            {
                //Debug.Log("Повышаем уровень навыка " + x.NewItem.Key + " уровень = " + x.NewItem.Value);
                var configId = x.NewItem.Key;
                var newLevel = x.NewItem.Value;
                UpdateParams(configId, newLevel);

                foreach (var skillEntity in _gameplayState.Skills)
                {
                    if (skillEntity.ConfigId != configId) continue;
                    skillEntity.Level.OnNext(newLevel);
                }
            }).AddTo(ref _disposables);
            //Кешируем бустеры для башен по типам Defence
            CalculateBoosters();

            //Подписка на те варианты, которые влияют на SkillViewModel 
            _fsmSkill.Fsm.StateCurrent.Subscribe(newState =>
            {
                //Debug.Log(newState.GetType());
                if (newState.GetType() == typeof(FsmSkillBegin))
                {
                    //Проходим все навыки и включаем или выключаем
                    foreach (var skillViewModel in _skillsMap) // ← список ПУСТ!
                    {
                        skillViewModel.IsActive.Value = skillViewModel.ConfigId == _fsmSkill.GetConfigId();
                    }
                }

                if (newState.GetType() == typeof(FsmSkillSetTarget))
                {
                }

                if (newState.GetType() == typeof(FsmSkillShowEffect))
                {
                    SetSkillEffect(_fsmSkill.GetConfigId()); //Применяем эффекты
                }

                if (newState.GetType() == typeof(FsmSkillNone))
                {
                    foreach (var skillViewModel in _skillsMap)
                    {
                        skillViewModel.IsActive.Value = false;
                    }
                }
            }).AddTo(ref _disposables);
        }

        private void UpdateParams(string configId, int level)
        {
            var levelSettings = _skillSettingsMap[configId].FirstOrDefault(l => l.Level == level);
            if (levelSettings == null) throw new Exception("Не найдены параметры башни " + configId);

            if (!SkillParametersMap.TryGetValue(configId, out var parameters)) return;

            foreach (var settingsParameter in levelSettings.Parameters)
            {
                if (parameters.TryGetValue(settingsParameter.ParameterType, out var parameter))
                {
                    parameter.Value *= 1 + settingsParameter.Value / 100;
                }
            }
        }

        public Dictionary<string, TypeEpic> GetAvailableSkills()
        {
            var skills = new Dictionary<string, TypeEpic>();

            foreach (var skillCard in _baseSkillCards)
            {
                skills.Add(skillCard.ConfigId, skillCard.EpicLevel);
            }

            return skills;
        }

        public Dictionary<ParameterType, ParameterData> GetParameters(string configId)
        {
            var skillViewModel = _skillsMap.FirstOrDefault(v => v.ConfigId == configId);
            if (skillViewModel == null) return null;
            return skillViewModel.Parameters;
        }
        /**
        * Список доступных навыков для апгрейда
        */
        public Dictionary<string, int> GetAvailableUpgradeSkills()
        {
            var towers = new Dictionary<string, int>();
            foreach (var skillViewModel in _skillsMap) //Все навыки
            {
                if (Levels[skillViewModel.ConfigId] < 3)
                    towers.TryAdd(skillViewModel.ConfigId, Levels[skillViewModel.ConfigId]); //Добавлять один раз
            }

            return towers;
        }

        private void SetSkillEffect(string configId)
        {
            var skillViewModel = _skillsMap.FirstOrDefault(v => v.ConfigId == configId);
            if (skillViewModel == null) return;

            skillViewModel.ToDestroy.Value = false;
            skillViewModel.EffectPosition.Value = _fsmSkill.Position.Value;
            skillViewModel.EffectDirection.Value = _fsmSkill.Direction.Value;
            
            _allSkills.Add(skillViewModel); //Запуск эффекта
            skillViewModel.ToDestroy
                .Where(x => x)
                .Take(1)
                .Subscribe(_ => { _allSkills.Remove(skillViewModel); })
                .AddTo(ref _disposables);
            
            //Запуск кулдауна
            skillViewModel?.StartCooldown();
            //Смена состояния
            _fsmSkill.Fsm.SetState<FsmSkillNone>();
        }
        
        public void StartSkill(string configId)
        {
            if (_fsmSkill.IsBegin() && _fsmSkill.GetConfigId() == configId)
            {
                _fsmSkill.Fsm.SetState<FsmSkillNone>();
            }
            else
            {
                _fsmSkill.Fsm.SetState<FsmSkillBegin>(configId);
            }
        }

        private void CalculateBoosters()
        {
            //MAINDO Проийти по списку все бустеры, и если есть совпадание с параметрами Навыка, увеличить
            var damageBooster = 0f;
            //бустеры общие
            if (_gameplayBoosters.SkillBust.TryGetValue(ParameterType.Damage, out var damage))
                damageBooster += damage;

            //бустеры от типа защиты и от наличия параметра в карточке
            foreach (var skillCard in _baseSkillCards)
            {
                //Фильтруем по наличию параметра в карточке навыка
                var isDamage = skillCard.Parameters.TryGetValue(ParameterType.Damage, out _) ||
                               skillCard.Parameters.TryGetValue(ParameterType.DPS, out _);

                var damageBoosterSkill = damageBooster;


                //бустеры от типа Defence о героя
                if (_gameplayBoosters.SkillDefenceBust.TryGetValue(skillCard.Defence, out var parameterDatas))
                {
                    if (parameterDatas.TryGetValue(ParameterType.Damage, out var damageDefence))
                        damageBoosterSkill += damageDefence;
                }

                Dictionary<ParameterType, float> boosters = new();

                if (isDamage && damageBoosterSkill != 0)
                {
                    boosters.Add(ParameterType.Damage, damageBoosterSkill);
                    boosters.Add(ParameterType.DPS, damageBoosterSkill);
                }

                SkillBoosters.Add(skillCard.ConfigId, boosters);
            }
        }

        public void Dispose()
        {
            SkillOne?.Dispose();
            SkillTwo?.Dispose();
            _disposables.Dispose();
        }

        public void LevelUpSkill(string configId)
        {
            var model = _skillsMap.Find(v => v.ConfigId == configId);
            if (model == null) return;
            Levels[configId] += 1;
        }
    }
}