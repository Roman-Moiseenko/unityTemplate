﻿using R3;
using Unity.VisualScripting;

namespace Game.State.GameResources
{
    public class Resource
    {
        public readonly ResourceData Origin;

        public readonly ReactiveProperty<int> Amount;
        public ResourceType ResourceType => Origin.ResourceType;

        public Resource(ResourceData data)
        {
            Origin = data;
            Amount = new ReactiveProperty<int>(data.Amount);

            Amount.Subscribe(newValue => data.Amount = newValue);
        }
    }
}