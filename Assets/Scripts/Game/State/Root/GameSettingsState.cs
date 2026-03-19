using System;

namespace Game.State.Root
{
    [Serializable]
    public class GameSettingsState
    {
        public string UserId { get; set; }
        public string UserToken { get; set; }
        public bool Vibration = false;
        public bool Damage = false;
        public bool Sound = false;
        public bool Music = false;
        
        public int MusicVolume;
        public int SFXVolume;

        public DateTime DateVersion;
    }
}