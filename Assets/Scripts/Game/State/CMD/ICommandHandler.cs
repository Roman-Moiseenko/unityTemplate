namespace Game.State.CMD
{
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        bool Handle(TCommand command);
        
    }
}