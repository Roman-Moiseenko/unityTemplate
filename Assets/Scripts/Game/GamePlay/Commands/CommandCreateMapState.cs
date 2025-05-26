using Game.State.CMD;

namespace Game.GamePlay.Commands
{
    public class CommandCreateMapState: ICommand
    {
        public readonly int MapId;

        public CommandCreateMapState(int mapId)
        {
            MapId = mapId;
        }
    }
}