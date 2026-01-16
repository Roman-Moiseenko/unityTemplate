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
            
            //TODO подключаем коллбеки на результат
            //Игра на паузе. Ожидаем удачного показа рекламы
            ad.SuccessShow();
            return ad;
            
        }
    }
}