using System;
using System.Collections.Generic;
using Game.Settings;


namespace MVVM.CMD
{
    public class QueryProcessor : IQueryProcessor
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly Dictionary<Type, object> _handlesMap = new();

        public QueryProcessor(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }
        
        public void RegisterHandler<TQuery>(IQueryHandler<TQuery> handler) where TQuery : IQuery
        {
            _handlesMap[typeof(TQuery)] = handler;
        }

        public object Request<TQuery>(TQuery query) where TQuery : IQuery
        {
            if (_handlesMap.TryGetValue(typeof(TQuery), out var handler))
            {
                var typeHandler = (IQueryHandler<TQuery>)handler;
                if (typeHandler == null)
                {
                    throw new Exception("Не загружен запрос " + typeof(TQuery));
                }
                return typeHandler.Handle(query, _settingsProvider);
            }
            throw new Exception("Не загружен запрос " + typeof(TQuery));

        }
        
    }
}