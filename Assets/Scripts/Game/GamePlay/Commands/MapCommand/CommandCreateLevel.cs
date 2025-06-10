using MVVM.CMD;

namespace Game.GamePlay.Commands.MapCommand
{
    public class CommandCreateLevel: ICommand
    {
        public readonly int MapId;

        public CommandCreateLevel(int mapId)
        {
            MapId = mapId;
        }
    }
}