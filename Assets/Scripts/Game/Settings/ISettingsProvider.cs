using System.Threading.Tasks;
using Game.State.Root;
using R3;

namespace Game.Settings
{
    public interface ISettingsProvider
    {
        GameSettings GameSettings { get; }
        ApplicationSettings ApplicationSettings { get; }

        public Observable<LoadingState> LoadGameSettings();
        

    }
}