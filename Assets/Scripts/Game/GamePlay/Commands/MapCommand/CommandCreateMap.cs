using Game.State.CMD;

namespace Game.GamePlay.Commands.MapCommand
{
    public class CommandCreateMap: ICommand
    {
        public readonly int MapId;

        public CommandCreateMap(int mapId)
        {
            MapId = mapId;
        }
    }
}