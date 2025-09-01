using MVVM.CMD;

namespace Game.GamePlay.Commands.MapCommand
{
    public class CommandCreateInfinity : ICommand
    {
        public int UniqueId;

        public CommandCreateInfinity(int uniqueId)
        {
            UniqueId = uniqueId;
        }
    }
}