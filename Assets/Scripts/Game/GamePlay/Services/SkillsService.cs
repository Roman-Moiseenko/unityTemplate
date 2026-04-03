using System.Collections.Generic;
using DI;
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
using ObservableCollections;

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

        public SkillsService(
            GameplayStateProxy gameplayState,
            SkillsSettings skillsSettings,
            GameplayEnterParams gameplayEnterParams,
            DIContainer container)
        {
            _container = container;
            _gameplayState = gameplayState;

            _cmd = container.Resolve<ICommandProcessor>();

            var skillEntities = gameplayState.Skills;
            _baseSkillCards = gameplayEnterParams.Skills; //Базовые настройки колоды
            _gameplayBoosters =
                gameplayEnterParams.GameplayBoosters; 
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
    }
}