using R3;

namespace Game.State.Root
{
    public class GameSettingsStateProxy
    {
        public readonly GameSettingsState Origin;
        public ReactiveProperty<string> UserId;
        public ReactiveProperty<string> UserToken;
        
        public ReactiveProperty<int> MusicVolume { get; }
        public ReactiveProperty<int> SFXVolume { get; }
        
        public GameSettingsStateProxy(GameSettingsState gameSettingsState)
        {
            Origin = gameSettingsState;
            UserId = new ReactiveProperty<string>(gameSettingsState.UserId);
            UserToken = new ReactiveProperty<string>(gameSettingsState.UserToken);
            
            MusicVolume = new ReactiveProperty<int>(gameSettingsState.MusicVolume);
            SFXVolume = new ReactiveProperty<int>(gameSettingsState.SFXVolume);

            MusicVolume.Skip(1).Subscribe(value => gameSettingsState.MusicVolume = value);
            SFXVolume.Skip(1).Subscribe(value => gameSettingsState.SFXVolume = value);

            UserId.Subscribe(value => gameSettingsState.UserId = value);
            UserToken.Subscribe(value => gameSettingsState.UserToken = value);
        }
    }
}