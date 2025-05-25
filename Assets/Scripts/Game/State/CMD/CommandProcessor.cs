using System;
using System.Collections.Generic;

namespace Game.State.CMD
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly IGameStateProvider _gameStateProvider;
        private readonly Dictionary<Type, object> _handlesMap = new();

        public CommandProcessor(IGameStateProvider gameStateProvider)
        {
            _gameStateProvider = gameStateProvider; //для сохранения игры
        }
        
        public void RegisterHandler<TCommand>(ICommandHandler<TCommand> handler) where TCommand : ICommand
        {
            _handlesMap[typeof(TCommand)] = handler;
        }

        /**
         * Сохранение процесса игры возможно будет нужно не везде, тогда можно создать
         * 2 процедуры: Process и ProcessNotSave / ProcessSave, либо добавить 2 параметр (..., bool saved = false) 
         */
        public bool Process<TCommand>(TCommand command) where TCommand : ICommand
        {
            if (_handlesMap.TryGetValue(typeof(TCommand), out var handler))
            {
                var typeHandler = (ICommandHandler<TCommand>)handler;
                var result = typeHandler.Handle(command);
                if (result) //Если команда успешно обработалась, то сохраняем состояние игры
                {
                    _gameStateProvider.SaveGameState();
                }
                return result;
            }
            return false;
        }
    }
}