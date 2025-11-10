using System;

namespace Game.State.Root
{
    [Serializable]
    public class GameSettingsState
    {
        public string UserId { get; set; }
        public string UserToken { get; set; }
        
        public int MusicVolume;
        public int SFXVolume;

        public DateTime DateVersion;
    }
}