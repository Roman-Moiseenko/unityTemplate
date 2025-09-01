using MVVM.CMD;

namespace Game.GamePlay.Commands.RoadCommand
{
    public class CommandRoadCreateBase : ICommand
    {
        public string RoadConfigId;
        public bool hasWaySecond;
        public bool hasWayDisabled;
    }
}