using System.Collections.Generic;
using DI;
using Game.GamePlay.Root;
using Game.GamePlay.View.Skills;
using Game.GamePlay.View.Towers;
using Game.Settings.Gameplay.Entities.Skill;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Common;
using Game.State.Gameplay;
using Game.State.Inventory.SkillCards;
using Game.State.Inventory.TowerCards;
using Game.State.Maps.Skills;
using Game.State.Maps.Towers;
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
        private readonly ObservableList<SkillViewModel> _allSkills = new();
        private readonly Dictionary<int, SkillViewModel> _skillsMap = new();
        
        private readonly Dictionary<string, List<TowerLevelSettings>> _towerSettingsMap = new();
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
            _gameplayBoosters = gameplayEnterParams.GameplayBoosters; //TODO Передать в башни _castleResearch.TowerDamage 

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
    }
}