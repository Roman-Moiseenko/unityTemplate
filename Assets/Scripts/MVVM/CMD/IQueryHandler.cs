using Game.Settings;

namespace MVVM.CMD
{
    public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
    {
        TResult Handle(TQuery query, ISettingsProvider settingsProvider);
        
    }
}