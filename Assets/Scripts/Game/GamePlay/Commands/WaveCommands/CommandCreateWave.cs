using System.Collections.Generic;
using Game.Settings.Gameplay.Enemies;
using MVVM.CMD;

namespace Game.GamePlay.Commands.WaveCommands
{
    public class CommandCreateWave : ICommand
    {
        public int Index;
        //public List<WaveItemSettings> WaveItems;
    }
}