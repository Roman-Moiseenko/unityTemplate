﻿using DI;
using Game.GamePlay.Services;
using Scripts.Game.GameRoot.Services;

namespace Game.GamePlay.Root.View
{
    /**
     * Регистрация view-моделей сцены
     */
    public static class GameplayViewModelsRegistrations
    {
        public static void Register(DIContainer container)
        {
            //Добавить сервис если нужен в UIGameplayRootViewModel и WorldGameplayRootViewModel
            container.RegisterFactory(c => new UIGameplayRootViewModel(container.Resolve<SomeGameplayService>())).AsSingle();
            container.RegisterFactory(c => new WorldGameplayRootViewModel()).AsSingle();
        }
    }
}