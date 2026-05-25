using System;
using R3;

namespace Game.State.Root
{
    public class GameSettingsStateProxy : IDisposable
    {
        public readonly GameSettingsState Origin;
        public ReactiveProperty<string> UserId;
        public ReactiveProperty<string> UserToken;

        public ReactiveProperty<bool> Vibration;
        public ReactiveProperty<bool> Damage;
        public ReactiveProperty<bool> Sound;
        public ReactiveProperty<bool> Music;
        
        public ReactiveProperty<int> MusicVolume { get; }
        public ReactiveProperty<int> SFXVolume { get; }

        private DisposableBag _disposables = new();

        public GameSettingsStateProxy(GameSettingsState gameSettingsState)
        {
            Origin = gameSettingsState;
            UserId = new ReactiveProperty<string>(gameSettingsState.UserId);
            UserToken = new ReactiveProperty<string>(gameSettingsState.UserToken);

            MusicVolume = new ReactiveProperty<int>(gameSettingsState.MusicVolume);
            SFXVolume = new ReactiveProperty<int>(gameSettingsState.SFXVolume);
            
            Vibration = new ReactiveProperty<bool>(gameSettingsState.Vibration);
            Damage = new ReactiveProperty<bool>(gameSettingsState.Damage);
            Sound = new ReactiveProperty<bool>(gameSettingsState.Sound);
            Music = new ReactiveProperty<bool>(gameSettingsState.Music);
            
            Vibration.Skip(1).Subscribe(value =>
            {
                gameSettingsState.Vibration = value;
                UpdateDateVersion();
            }).AddTo(ref _disposables);
            Damage.Skip(1).Subscribe(value =>
            {
                gameSettingsState.Damage = value;
                UpdateDateVersion();
            }).AddTo(ref _disposables);
            Sound.Skip(1).Subscribe(value =>
            {
                gameSettingsState.Sound = value;
                UpdateDateVersion();
            }).AddTo(ref _disposables);
            Music.Skip(1).Subscribe(value =>
            {
                gameSettingsState.Music = value;
                UpdateDateVersion();
            }).AddTo(ref _disposables);
            MusicVolume.Skip(1).Subscribe(value =>
            {
                gameSettingsState.MusicVolume = value;
                UpdateDateVersion();
            }).AddTo(ref _disposables);
            SFXVolume.Skip(1).Subscribe(value =>
            {
                gameSettingsState.SFXVolume = value;
                UpdateDateVersion();
            }).AddTo(ref _disposables);
            UserId.Subscribe(value =>
            {
                gameSettingsState.UserId = value;
                UpdateDateVersion();
            }).AddTo(ref _disposables);
            UserToken.Subscribe(value => gameSettingsState.UserToken = value).AddTo(ref _disposables);
        }

        private void UpdateDateVersion()
        {
            Origin.DateVersion = DateTime.Now;
        }

        public void Dispose()
        {
            _disposables.Dispose();
            UserId?.Dispose();
            UserToken?.Dispose();
            Vibration?.Dispose();
            Damage?.Dispose();
            Sound?.Dispose();
            Music?.Dispose();
            MusicVolume?.Dispose();
            SFXVolume?.Dispose();
        }
    }
}