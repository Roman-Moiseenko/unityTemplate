namespace MVVM.CMD
{
    public interface IQueryProcessor
    {
        void RegisterHandler<TQuery>(IQueryHandler<TQuery> handler) where TQuery: IQuery;
        object Request<TQuery>(TQuery query) where TQuery : IQuery;
    }
}