using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.SkillStates;
using Game.GamePlay.Root;
using Game.GamePlay.View.Skills;
using Game.Settings.Gameplay.Entities.Skill;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Common;
using Game.State.Gameplay;
using Game.State.Inventory.SkillCards;
using Game.State.Maps.Skills;
using Game.State.Research;
using MVVM.CMD;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class SkillsService
    {
        public readonly Dictionary<string,
            Dictionary<SkillParameterType, SkillParameterData>> SkillParametersMap = new();

        public IObservableCollection<SkillViewModel> AllSkills =>
            _allSkills; //Интерфейс менять нельзя, возвращаем через динамический массив

        public readonly ObservableDictionary<string, int> Levels = new(); //Уровни башен по типам ConfigId

        private readonly List<SkillCardData> _baseSkillCards; //
        private readonly ICommandProcessor _cmd;
        
        //УДалить
        private readonly ObservableList<SkillViewModel> _allSkills = new();
        private readonly Dictionary<int, SkillViewModel> _skillsMap = new();

        //Использовать 
        public SkillViewModel SkillOne;
        public SkillViewModel SkillTwo;
        
        private readonly Dictionary<string, List<SkillLevelSettings>> _skillSettingsMap = new();
        private readonly GameplayStateProxy _gameplayState;
        private readonly DIContainer _container;
        private readonly GameplayBoosters _gameplayBoosters;

        public Dictionary<string, Dictionary<SkillParameterType, float>> SkillBoosters = new();
        private readonly FsmSkill _fsmSkill;

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
            
            //Кешируем настройки зданий / объектов
            foreach (var skillSettings in skillsSettings.AllSkills)
            {
                _skillSettingsMap[skillSettings.ConfigId] = skillSettings.GameplayLevels;
                Levels[skillSettings.ConfigId] = 1;
            }
            //Кешируем уровень башни по конфигу, если башня этого типа есть на карте
            foreach (var skillEntity in _gameplayState.Skills)
            {
                Levels[skillEntity.ConfigId] = skillEntity.Level.CurrentValue;
            }
            
            
            foreach (var skillCardData in  _baseSkillCards)
            {
                var param = new Dictionary<SkillParameterType, SkillParameterData>();
                foreach (var parameterData in skillCardData.Parameters)
                {
                    param.Add(parameterData.Key, parameterData.Value.GetCopy());
                }
                SkillParametersMap.Add(skillCardData.ConfigId, param);
                for (var i = 1; i <= Levels[skillCardData.ConfigId]; i++)
                {
                    UpdateParams(skillCardData.ConfigId, i); //Увеличиваем параметры по геймплей уровню башни
                }
            }
            
            
            if (_gameplayState.Skills.Count > 0)
            {
                _gameplayState.Skills[0].Parameters = SkillParametersMap[_gameplayState.Skills[0].ConfigId]; 
                SkillOne = new SkillViewModel(_gameplayState.Skills[0], this);
                _allSkills.Add(SkillOne);
            }
            if (_gameplayState.Skills.Count > 1)
            {
                _gameplayState.Skills[1].Parameters = SkillParametersMap[_gameplayState.Skills[1].ConfigId]; 
                SkillTwo = new SkillViewModel(_gameplayState.Skills[1], this);
                _allSkills.Add(SkillTwo);
            }
            
            
            _gameplayState.Skills.ObserveAdd().Subscribe(e =>
            {
                var skillEntity = e.Value;
                skillEntity.Level.Value = Levels[skillEntity.ConfigId]; //Устанавливаем уровень апгрейда
                skillEntity.Parameters = SkillParametersMap[skillEntity.ConfigId];
                if (_gameplayState.Skills.Count > 2) throw new Exception("Ошибка");
                
                if (SkillOne != null && SkillTwo == null)
                {
                    SkillTwo = new SkillViewModel(skillEntity, this);
                    _allSkills.Add(SkillTwo);
                }
                if (SkillOne == null)
                {
                    SkillOne = new SkillViewModel(skillEntity, this);
                    _allSkills.Add(SkillOne);
                }

            });
            Levels.ObserveChanged().Subscribe(x =>
            {
                var configId = x.NewItem.Key;
                var newLevel = x.NewItem.Value;
                UpdateParams(configId, newLevel);

                foreach (var skillEntity in _gameplayState.Skills)
                {
                    if (skillEntity.ConfigId != configId) continue;
                    skillEntity.Level.OnNext(newLevel);
                }
            });
            //Кешируем бустеры для башен по типам Defence
            CalculateBoosters();
            
         //   var skillEntities = gameplayState.Skills;
            

            


            
//            Debug.Log(JsonConvert.SerializeObject(SkillOne, Formatting.Indented));
            
         //   Debug.Log(JsonConvert.SerializeObject(gameplayState.SkillOne, Formatting.Indented));
            //Создаем из настроек модели
            
         //   Debug.Log(JsonConvert.SerializeObject(gameplayState.EnterParams, Formatting.Indented));
         
            //Подписка на те варианты, которые влияют на SkillViewModel 
            _fsmSkill.Fsm.StateCurrent.Subscribe(newState =>
            {
                //Debug.Log(newState.GetType());
                if (newState.GetType() == typeof(FsmSkillBegin))
                {
                    //Проходим все навыки и включаем или выключаем
                    foreach (var skillViewModel  in _allSkills)
                    {
                        skillViewModel.IsActive.Value = skillViewModel.ConfigId == _fsmSkill.GetConfigId();
                    }
                }

                if (newState.GetType() == typeof(FsmSkillSetTarget))
                {
                    
                }

                if (newState.GetType() == typeof(FsmSkillShowEffect))
                {
                    Debug.Log("FsmSkillShowEffect");
                    SetSkillEffect(_fsmSkill.GetConfigId()); //Применяем эффекты
                }

                if (newState.GetType() == typeof(FsmSkillNone))
                {
                    foreach (var skillViewModel  in _allSkills)
                    {
                        skillViewModel.IsActive.Value = false;
                    }
                }
                
            });         
             
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
        
        private void CreateSkillViewModel(SkillEntity skillEntity)
        {
            var skillViewModel = new SkillViewModel(skillEntity, this);
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

        /**
        * Список доступных навыков для апгрейда
        */
        public Dictionary<string, int> GetAvailableUpgradeSkills()
        {
            var towers = new Dictionary<string, int>();
            foreach (var skillViewModel in _allSkills) //Все навыки
            {
                if (Levels[skillViewModel.ConfigId] < 3)
                    towers.TryAdd(skillViewModel.ConfigId, Levels[skillViewModel.ConfigId]); //Добавлять один раз
            }

            return towers;
        }

        private void SetSkillEffect(string configId)
        {
            var skillViewModel = _allSkills.FirstOrDefault(v => v.ConfigId == configId);
            //TODO Запуск эффекта
            
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
            } else
            {
                _fsmSkill.Fsm.SetState<FsmSkillBegin>(configId);
            }
        }
        
        private void CalculateBoosters()
        {

            //бустеры общие
            var damageBooster = _gameplayBoosters.SkillDamage;

            //бустеры общие от героя
            if (_gameplayBoosters.HeroSkillBust.TryGetValue(SkillParameterType.Damage, out var damage))
                damageBooster += damage;

            
            //бустеры от типа защиты и от наличия параметра в карточке
            foreach (var skillCard in _baseSkillCards)
            {
                //Фильтруем по наличию параметра в карточке навыка
                var isDamage = skillCard.Parameters.TryGetValue(SkillParameterType.Damage, out _) ||
                               skillCard.Parameters.TryGetValue(SkillParameterType.DPS, out _);

                var damageBoosterSkill = damageBooster;

                
                //бустеры от типа Defence о героя
                if (_gameplayBoosters.HeroSkillDefenceBust.TryGetValue(skillCard.Defence, out var parameterDatas))
                {
                    if (parameterDatas.TryGetValue(SkillParameterType.Damage, out var damageDefence))
                        damageBoosterSkill += damageDefence;

                }

                Dictionary<SkillParameterType, float> boosters = new(); 
                
                if (isDamage && damageBoosterSkill != 0)
                {
                    boosters.Add(SkillParameterType.Damage, damageBoosterSkill);
                    boosters.Add(SkillParameterType.DPS, damageBoosterSkill);
                }  
              
                SkillBoosters.Add(skillCard.ConfigId, boosters);
            }
        }
    }
}