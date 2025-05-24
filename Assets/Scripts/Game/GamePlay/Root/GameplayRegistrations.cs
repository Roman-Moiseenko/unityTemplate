using DI;
using Game.GamePlay.Services;
using Scripts.Game.GameRoot.Services;

namespace Game.GamePlay.Root
{
    public static class GameplayRegistrations
    {
        /**
         * Регистрируем все сервисы для сцены, вручную 
         */
        public static void Register(DIContainer container, GameplayEnterParams gameplayEnterParams)
        {
            container.RegisterFactory(c => new SomeGameplayService(c.Resolve<SomeCommonService>())).AsSingle();
        }
        
        
    }
}