using System.Collections;
using DI;
using Scripts.Game.GameRoot.Entity;
using Scripts.Utils;
using UnityEngine;

namespace Game.GameRoot.Services
{
    public class AdService
    {
        private readonly DIContainer _container;

        private Coroutines _coroutines;
        
        public AdService(DIContainer container)
        {
            _container = container;
            _coroutines = GameObject.Find("[COROUTINES]").GetComponent<Coroutines>();
        }


        public AdGoogle ShowAdGoogle()
        {

            //TODO Запускаем рекламу
            var ad = new AdGoogle();
            
            //TODO подключаем коллбеки на результат, для теста корутин на 1 с


            _coroutines.StartCoroutine(ShowAdTemp(ad));
            return ad;
            
        }

        private IEnumerator ShowAdTemp(AdGoogle ad)
        {
            yield return new WaitForSeconds(1f);
            ad.SuccessShow();
        }
    }
}