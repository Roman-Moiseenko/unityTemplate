using Game.State.Common;
using MVVM.CMD;

namespace Game.MainMenu.Commands.SkillCommands
{
    public class CommandSkillCardAdd : ICommand
    {
        public string ConfigId;
        public TypeEpic EpicLevel;
        public int Level;
    }
}