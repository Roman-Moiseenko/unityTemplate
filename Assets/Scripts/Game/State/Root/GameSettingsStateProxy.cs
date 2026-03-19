using System;
using R3;

namespace Game.State.Root
{
    public class GameSettingsStateProxy
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
            });
            Damage.Skip(1).Subscribe(value =>
            {
                gameSettingsState.Damage = value;
                UpdateDateVersion();
            });
            Sound.Skip(1).Subscribe(value =>
            {
                gameSettingsState.Sound = value;
                UpdateDateVersion();
            });
            Music.Skip(1).Subscribe(value =>
            {
                gameSettingsState.Music = value;
                UpdateDateVersion();
            });
            
            MusicVolume.Skip(1).Subscribe(value =>
            {
                gameSettingsState.MusicVolume = value;
                UpdateDateVersion();
            });
            SFXVolume.Skip(1).Subscribe(value =>
            {
                gameSettingsState.SFXVolume = value;
                UpdateDateVersion();
            });

            UserId.Subscribe(value =>
            {
                gameSettingsState.UserId = value;
                UpdateDateVersion();
            });
            UserToken.Subscribe(value => gameSettingsState.UserToken = value);
        }

        private void UpdateDateVersion()
        {
            Origin.DateVersion = DateTime.Now;
        }
    }
}