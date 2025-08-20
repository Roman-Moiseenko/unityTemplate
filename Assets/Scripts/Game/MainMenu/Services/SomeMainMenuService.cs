using System;
using Game.GameRoot.Services;
using UnityEngine;

namespace Game.MainMenu.Services
{
    public class SomeMainMenuService //: IDisposable
    {
        private readonly SomeCommonService _someCommonService;

        public SomeMainMenuService(SomeCommonService someCommonService)
        {
            _someCommonService = someCommonService;
            Debug.Log(GetType().Name + " был создан");
        }

      /*  public void Dispose()
        {
            Debug.Log("Чистка подписок");
        }*/
    }
}