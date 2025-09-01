using Game.Settings;
using Game.State.Maps.Castle;
using Game.State.Root;
using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.CastleCommands
{
    public class CommandCastleCreateHandler : ICommandHandler<CommandCastleCreate>
    {
        private readonly GameSettings _gameSettings;
        private readonly GameplayStateProxy _gameplayState;

        public CommandCastleCreateHandler(GameSettings gameSettings, GameplayStateProxy gameplayState)
        {
            _gameSettings = gameSettings;
            _gameplayState = gameplayState;
        }
        public bool Handle(CommandCastleCreate command)
        {
            var castle = new CastleEntityData()
            {
                ConfigId = "Castle",
                Position = new Vector2Int(0, 0),
                //Базовые параметры
                Damage = _gameSettings.CastleInitialSettings.Damage,
                FullHealth = _gameSettings.CastleInitialSettings.FullHealth,
                Speed = _gameSettings.CastleInitialSettings.Speed,
                ReduceHealth = _gameSettings.CastleInitialSettings.ReduceHealth,
                CurrenHealth = _gameSettings.CastleInitialSettings.FullHealth,
            };
            
            _gameplayState.Castle.CastleEntityInitialization(castle);
            
            return false;
        }
    }
}