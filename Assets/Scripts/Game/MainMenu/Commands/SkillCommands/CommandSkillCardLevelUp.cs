using MVVM.CMD;

namespace Game.MainMenu.Commands.SkillCommands
{
    public class CommandSkillCardLevelUp : ICommand
    {
        public int UniqueId;

        public CommandSkillCardLevelUp(int uniqueId)
        {
            UniqueId = uniqueId;
        }
    }
}