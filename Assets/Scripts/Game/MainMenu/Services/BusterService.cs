using System;
using Game.Settings;
using Game.State.Root;
using MVVM.CMD;
using R3;

namespace Game.MainMenu.Services
{
    public class BusterService : IDisposable
    {
        private DisposableBag _disposables = new();
        private readonly GameStateProxy _gameState;
        private readonly ICommandProcessor _cmd;
        private readonly GameSettings _gameSettings;


        public BusterService(
            GameStateProxy gameState,
            ICommandProcessor cmd,
            GameSettings gameSettings)
        {
            _gameState = gameState;
            _cmd = cmd;
            _gameSettings = gameSettings;
        }
        
        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}