using System;
using System.Collections.Generic;
using System.Linq;
using Game.Settings;
using Game.State.Inventory.SkillCards;
using Game.State.Maps.Skills;
using Game.State.Root;
using MVVM.CMD;

namespace Game.MainMenu.Commands.SkillCommands
{
    public class CommandSkillCardAddHandler : ICommandHandler<CommandSkillCardAdd>
    {
        private readonly GameStateProxy _gameState;
        private readonly GameSettings _gameSettings;

        public CommandSkillCardAddHandler(GameStateProxy gameState, GameSettings gameSettings)
        {
            _gameState = gameState;
            _gameSettings = gameSettings;
        }
        public bool Handle(CommandSkillCardAdd command)
        {
            var skills = _gameSettings.SkillsSettings.AllSkills;
            var skillConfig = skills.FirstOrDefault(t => t.ConfigId == command.ConfigId);
            
            if (skillConfig == null) throw new Exception($"skillConfig = {command.ConfigId}  Not Find");

            var initialSkillCard = new SkillCardData
            {
                UniqueId = _gameState.CreateInventoryID(),
                ConfigId = command.ConfigId,
                EpicLevel = command.EpicLevel,
                Level = command.Level,
                Amount = 1, 
                Parameters = new Dictionary<SkillParameterType, SkillParameterData>(),
                Defence = skillConfig.Defence,
                TypeTarget = skillConfig.TypeTarget,
            };
               
            foreach (var baseParameter in skillConfig.BaseParameters)
            {
                initialSkillCard.Parameters.Add(baseParameter.ParameterType, new SkillParameterData(baseParameter));
            }
            
            _gameState.Inventory.AddItem(initialSkillCard);
            return true;
        }
    }
}