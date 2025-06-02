using System;
using System.Collections.Generic;
using Game.GamePlay.Commands;
using Game.GamePlay.View.GameResources;
using Game.State.CMD;
using Game.State.GameResources;
using ObservableCollections;
using R3;

namespace Game.GamePlay.Services
{
    public class ResourcesService
    {
        public readonly ObservableList<ResourceViewModel> Resources = new();
        private readonly Dictionary<ResourceType, ResourceViewModel> _resourcesMap = new();

        private readonly ICommandProcessor _cmd;

        public ResourcesService(ObservableList<Resource> resources, ICommandProcessor cmd)
        {
            _cmd = cmd;
            resources.ForEach(resource => CreateResourceViewModel(resource));
            resources.ObserveAdd().Subscribe(e => CreateResourceViewModel(e.Value));
            resources.ObserveRemove().Subscribe(e => RemoveResourceViewModel(e.Value));
        }

        public bool AddResource(ResourceType resourceType, int amount)
        {
            var command = new CommandResourcesAdd(resourceType, amount);
            return _cmd.Process(command);
        }
        
        public bool TrySpendResource(ResourceType resourceType, int amount)
        {
            var command = new CommandResourcesSpend(resourceType, amount);
            return _cmd.Process(command);
        }

        /**
         * Проверка на наличие достаточного кол-ва ресурса
         */
        public bool IsEnoughResources(ResourceType resourceType, int amount)
        {
            if (_resourcesMap.TryGetValue(resourceType, out var resourceViewModel))
            {
                return resourceViewModel.Amount.CurrentValue >= amount;
            }
            return false;
        }

        /**
         * Метод для подписания на ресурс, когда он меняется, возвращает кол-во
         */
        public Observable<int> ObservableResource(ResourceType resourceType)
        {
            if (_resourcesMap.TryGetValue(resourceType, out var resourceViewModel))
            {
                return resourceViewModel.Amount;
            }

            throw new Exception($"Resource of  type {resourceType} dosn't exist");
        }

        private void CreateResourceViewModel(Resource resource)
        {
            var resourceViewModal = new ResourceViewModel(resource);
            _resourcesMap[resource.ResourceType] = resourceViewModal;
            Resources.Add(resourceViewModal);
        }

        private void RemoveResourceViewModel(Resource resource)
        {
            if (_resourcesMap.TryGetValue(resource.ResourceType, out var resourceViewModel))
            {
                Resources.Remove(resourceViewModel);
                _resourcesMap.Remove(resource.ResourceType);
            }
        }
    }
}