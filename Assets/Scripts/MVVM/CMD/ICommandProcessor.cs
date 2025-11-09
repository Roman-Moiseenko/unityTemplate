namespace MVVM.CMD
{
    public interface ICommandProcessor
    {
        void RegisterHandler<TCommand>(ICommandHandler<TCommand> handler) where TCommand: ICommand;
        bool Process<TCommand>(TCommand command, bool autoSave = true) where TCommand : ICommand;
    }
}