using UnityEngine;

namespace Scripts.Game.GameRoot.Services
{
    /**
     * Главные сервисы проекта:
     * 
     * Провайдер состояни, или настроек, сервис аналитики и т.д.
     */
    public class SomeCommonService
    {
        public SomeCommonService()
        {
            Debug.Log(GetType().Name + " был  создан");
        }
    }
}