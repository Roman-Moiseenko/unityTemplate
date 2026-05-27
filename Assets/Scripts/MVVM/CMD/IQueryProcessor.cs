namespace MVVM.CMD
{
    public interface IQueryProcessor
    {
        void RegisterHandler<TQuery, TResult>(IQueryHandler<TQuery, TResult> handler) 
            where TQuery : IQuery<TResult>;
        TResult Request<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>;
    }
}