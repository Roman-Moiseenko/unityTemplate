using MVVM.CMD;

namespace Game.GamePlay.Commands.MobCommands
{
    public class CommandCreateMob : ICommand
    {
        public string ConfigId;
        public int Level;
        public int NumberWave;
        public int Quantity;
    }
}