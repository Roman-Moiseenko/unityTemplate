using System;
using System.Linq;
using Game.Settings;
using Game.State.Inventory.HeroCards;
using Game.State.Parameters;
using Game.State.Root;
using MVVM.CMD;

namespace Game.MainMenu.Commands.HeroCommands
{
    public class CommandHeroCardAddHandler : ICommandHandler<CommandHeroCardAdd>
    {
        
        private readonly GameStateProxy _gameState;
        private readonly GameSettings _gameSettings;

        public CommandHeroCardAddHandler(GameStateProxy gameState, GameSettings gameSettings)
        {
            _gameState = gameState;
            _gameSettings = gameSettings;
        }
        
        public bool Handle(CommandHeroCardAdd command)
        {
            var heroes = _gameSettings.HeroesSettings.AllHeroes;
            var heroConfig = heroes.FirstOrDefault(t => t.ConfigId == command.ConfigId);
            
            if (heroConfig == null) throw new Exception($"heroConfig = {command.ConfigId}  Not Find");
            
            //Проверка, есть ли уже такой герой в Инвентаре (Будем использовать при обновлении настроек, )
            if (_gameState.Inventory.Items.FirstOrDefault(item => item.ConfigId == command.ConfigId) != null) return false;
            
            var initialHeroCard = new HeroCardData
            {
                UniqueId = _gameState.CreateInventoryID(),
                Available = command.Available,
                ConfigId = command.ConfigId,
                Level = command.Level,
                Rank = command.Rank,
                Amount = 1,
                EpicLevel = heroConfig.Epic,
                Defence =  heroConfig.Defence,
                Name = heroConfig.TitleLid,
            };
            
            foreach (var baseParameter in heroConfig.BaseParameters)
            {
                initialHeroCard.Parameters.Add(baseParameter.ParameterType, new ParameterData(baseParameter));
            }
            
            _gameState.Inventory.AddItem(initialHeroCard);
            
            return  true;
        }
    }
}