using System;
using System.Collections.Generic;
using Game.State;

namespace MVVM.CMD
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly IGameStateProvider _gameStateProvider;
        private readonly Dictionary<Type, object> _handlesMap = new();

        //Командный процессор получает ссылку на репозиторий, для сохранения данных в процесссе обработки команд
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
        public bool Process<TCommand>(TCommand command, bool autoSave = true) where TCommand : ICommand
        {
        //    UnityEngine.Debug.Log("Process " + command.GetType());
            if (_handlesMap.TryGetValue(typeof(TCommand), out var handler))
            {
                var typeHandler = (ICommandHandler<TCommand>)handler;
                if (typeHandler == null)
                {
                    throw new Exception("Не загружена команда " + typeof(TCommand));
                }
                var result = typeHandler.Handle(command);
                if (result && autoSave) //Если команда успешно обработалась, то сохраняем состояние игры
                {
                    _gameStateProvider.SaveGameState();
                }
                return result;
            }
            throw new Exception("Не загружена команда " + typeof(TCommand));
            //TODO на продакшн убрать исключения
            //return false;
        }
    }
}