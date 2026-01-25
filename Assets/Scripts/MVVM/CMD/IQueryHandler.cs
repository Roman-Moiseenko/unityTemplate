using Game.Settings;

namespace MVVM.CMD
{
    public interface IQueryHandler<TQuery> where TQuery : IQuery
    {
        object Handle(TQuery query, ISettingsProvider settingsProvider);
        
    }
}