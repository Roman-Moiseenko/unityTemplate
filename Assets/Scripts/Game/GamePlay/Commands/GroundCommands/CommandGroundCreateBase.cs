using MVVM.CMD;

namespace Game.GamePlay.Commands.GroundCommands
{
    public class CommandGroundCreateBase : ICommand
    {
        public bool IsSmall = false;
        public string GroundConfigId;
        public bool Obstacle = false; //Препятствия
        public int Collapse = 0; //Степень провалов на начальной карте
    }
}