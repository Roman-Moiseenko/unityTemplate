using Game.GamePlay.Services;
using Game.GamePlay.View.Buildins;
using ObservableCollections;

namespace Game.GamePlay.Root.View
{
    public class WorldGameplayRootViewModel
    {
        public readonly IObservableCollection<BuildingViewModel> AllBuildings;

        public WorldGameplayRootViewModel(BuildingsService buildingsService)
        {
            AllBuildings = buildingsService.AllBuildings;
        }

    }
}