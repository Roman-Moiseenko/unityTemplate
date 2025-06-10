using System;
using System.Collections.Generic;

namespace Game.State.CMD
{
    public class CommandProcessorGameplay : ICommandProcessor
    {
        private readonly IGameStateProvider _gameStateProvider;
        private readonly Dictionary<Type, object> _handlesMap = new();

        //Командный процессор получает ссылку на репозиторий, для сохранения данных в процесссе обработки команд
        public CommandProcessorGameplay(IGameStateProvider gameStateProvider)
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
                if (result) //Если команда успешно обработалась, то сохраняем состояние сессии игры
                {
                    _gameStateProvider.SaveGameplayState();
                }
                return result;
            }
            return false;
        }
    }
}