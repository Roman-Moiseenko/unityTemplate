using MVVM.CMD;

namespace Game.GamePlay.Commands.WaveCommands
{
    /**
     * Генерация волны для бесконечной игры, пока не используется
     */
    public class CommandWaveGenerate : ICommand
    {
        public int Wave;

        public CommandWaveGenerate(int wave)
        {
            Wave = wave;
        }
    }
}