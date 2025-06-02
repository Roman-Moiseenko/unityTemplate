using Game.GamePlay.Services;
using Game.GamePlay.View.Buildins;
using ObservableCollections;

namespace Game.GamePlay.Root.View
{
    public class WorldGameplayRootViewModel
    {
        private readonly ResourcesService _resourcesService;
        //public readonly IObservableCollection<BuildingViewModel> AllBuildings;

        public WorldGameplayRootViewModel(/*BuildingsService buildingsService,*/ ResourcesService resourcesService)
        {
            _resourcesService = resourcesService;
            
            //AllBuildings = buildingsService.AllBuildings;
        }

    }
}