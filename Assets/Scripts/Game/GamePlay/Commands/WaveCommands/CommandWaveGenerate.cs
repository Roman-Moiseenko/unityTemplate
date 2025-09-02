using MVVM.CMD;

namespace Game.GamePlay.Commands.WaveCommands
{
    public class CommandWaveGenerate : ICommand
    {
        public int Wave;

        public CommandWaveGenerate(int wave)
        {
            Wave = wave;
        }
    }
}