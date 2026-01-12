
using System;
using System.Collections;
using R3;
using UnityEngine;

namespace Game.Common
{
    internal static class ObservableExtensions
    {

        public static Observable<T> FromCoroutine<T>(this MonoBehaviour monoBehaviour, Func<Observer<T>, IEnumerator> coroutineFactory)
        {
            return Observable.Create<T>(observer =>
            {
                var coroutine = coroutineFactory(observer);
                monoBehaviour.StartCoroutine(coroutine);

                // Возвращаем Disposable, который останавливает корутину при отписке
                return Disposable.Create(() => monoBehaviour.StopCoroutine(coroutine));
            });
        }
        
        public static Observable<Unit> FromCoroutine(this MonoBehaviour monoBehaviour, Func<IEnumerator> coroutineFactory)
        {

            return Observable.Create<Unit>(observer =>
            {
                var coroutine = coroutineFactory();
                var wrappedCoroutine = WrapCoroutineForCompletion(coroutine, observer);

                monoBehaviour.StartCoroutine(wrappedCoroutine);

                return Disposable.Create(() => monoBehaviour.StopCoroutine(coroutine));
            });
        }
        
        private static IEnumerator WrapCoroutineForCompletion(IEnumerator originalCoroutine, Observer<Unit> observer)
        {
            while (true)
            {
                try
                {
                    if (!originalCoroutine.MoveNext())
                    {
                        // Корутина завершилась успешно
                        observer.OnCompleted();
                        yield break; // Выход из обертки
                    }
                }
                catch (Exception ex)
                {
                    // Если в корутине произошла ошибка, передаем ее подписчику
                    observer.OnErrorResume(ex);
                    yield break; // Выход после ошибки
                }
                yield return originalCoroutine.Current;
            }
        
        }
    }
}