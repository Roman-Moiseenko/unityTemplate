using System;
using Scripts.Game.GameRoot.Services;
using UnityEngine;

namespace Game.GamePlay.Services
{
    /**
     * Внутренний сервис геймплея, зависящий от сервисов проекта
     */
    public class SomeGameplayService: IDisposable
    {
        private readonly SomeCommonService _someCommonService;
        
        public SomeGameplayService(SomeCommonService someCommonService)
        {
            _someCommonService = someCommonService;
            Debug.Log(GetType().Name + " был создан");
        }

        public void Dispose()
        {
            Debug.Log("Чистка подписок");
        }
    }
}