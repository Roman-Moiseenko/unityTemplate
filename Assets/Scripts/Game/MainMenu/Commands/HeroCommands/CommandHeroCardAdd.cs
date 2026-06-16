using Game.State.Common;
using MVVM.CMD;

namespace Game.MainMenu.Commands.HeroCommands
{
    public class CommandHeroCardAdd : ICommand
    {
        public string ConfigId;
        public bool Available;
        public int Level;
        public int Rank;
    }
}