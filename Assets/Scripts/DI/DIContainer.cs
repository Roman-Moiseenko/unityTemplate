using System;
using System.Collections.Generic;

namespace DI
{
public class DIContainer : IDisposable
    {
        private readonly DIContainer _parentContainer;
        private readonly Dictionary<(string, Type), DIEntry> _entriesMap = new();
        private readonly HashSet<(string, Type)> _resolutionsCache = new();
        private readonly List<IDisposable> _sceneDisposables = new();

        public DIContainer(DIContainer parentContainer = null)
        {
            _parentContainer = parentContainer;
        }

        public DIEntry RegisterFactory<T>(Func<DIContainer, T> factory)
        {
            return RegisterFactory(null, factory, false);
        }

        public DIEntry RegisterFactory<T>(string tag, Func<DIContainer, T> factory)
        {
            return RegisterFactory(tag, factory, false);
        }
        /// <summary>
        /// Регистрирует фабрику для создания T.
        /// </summary>
        /// <param name="tag">Необязательный тег для именованной регистрации.</param>
        /// <param name="factory">Фабрика, принимающая контейнер и возвращающая T.</param>
        /// <param name="overrideIfExists">
        /// Если true — перезаписывает существующую регистрацию (tag, T) без исключения.
        /// Полезно в дочерних контейнерах для переопределения сервисов родителя.
        /// Если false — кидает исключение при повторной регистрации (поведение по умолчанию).
        /// </param>
        public DIEntry RegisterFactory<T>(string tag, Func<DIContainer, T> factory, bool overrideIfExists)
        {
            var key = (tag, typeof(T));

            if (!overrideIfExists && _entriesMap.ContainsKey(key))
            {
                throw new Exception(
                    $"DI: Factory with tag {key.Item1} and type {key.Item2.FullName} has already registered");
            }

            var diEntry = new DIEntry<T>(this, factory);
            _entriesMap[key] = diEntry;

            return diEntry;
        }

        public void RegisterInstance<T>(T instance)
        {
            RegisterInstance(null, instance, false);
        }

        public void RegisterInstance<T>(string tag, T instance)
        {
            RegisterInstance(tag, instance, false);
        }
        /// <summary>
        /// Регистрирует готовый экземпляр T.
        /// </summary>
        /// <param name="tag">Необязательный тег для именованной регистрации.</param>
        /// <param name="instance">Готовый экземпляр.</param>
        /// <param name="overrideIfExists">
        /// Если true — перезаписывает существующую регистрацию (tag, T) без исключения.
        /// Если false — кидает исключение при повторной регистрации (поведение по умолчанию).
        /// </param>
        public void RegisterInstance<T>(string tag, T instance, bool overrideIfExists)
        {
            var key = (tag, typeof(T));

            if (!overrideIfExists && _entriesMap.ContainsKey(key))
            {
                throw new Exception(
                    $"DI: Instance with tag {key.Item1} and type {key.Item2.FullName} has already registered");
            }

            var diEntry = new DIEntry<T>(instance);
            _entriesMap[key] = diEntry;
        }

        public T Resolve<T>(string tag = null)
        {
            var key = (tag, typeof(T));

            if (_resolutionsCache.Contains(key))
            {
                throw new Exception($"DI: Cyclic dependency for tag {key.tag} and type {key.Item2.FullName}");
            }

            _resolutionsCache.Add(key);

            try
            {
                if (_entriesMap.TryGetValue(key, out var diEntry))
                {
                    return diEntry.Resolve<T>();
                }

                if (_parentContainer != null)
                {
                    // Рекурсивный вызов родительского контейнера.
                    // Если в цепочке родителей произойдёт исключение,
                    // finally внешнего блока гарантированно очистит _resolutionsCache
                    // текущего контейнера (finally выполняется до раскрутки стека исключения).
                    return _parentContainer.Resolve<T>(tag);
                }
            }
            finally
            {
                _resolutionsCache.Remove(key);
            }

            // Если запросили Lazy<T>, а зарегистрирован T — автоматически оборачиваем
            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Lazy<>))
            {
                var valueType = typeof(T).GetGenericArguments()[0];
                if (IsRegistered(valueType, tag))
                {
                    return (T)Activator.CreateInstance(
                        typeof(Lazy<>).MakeGenericType(valueType),
                        new Func<object>(() => ResolveUntyped(valueType, tag)));
                }
            }

            throw new Exception($"Couldn't find dependency for tag {tag} and type {key.Item2.FullName}");
        }

        /// <summary>
        /// Проверяет, зарегистрирован ли тип (с опциональным тегом) в текущем контейнере или родителях.
        /// </summary>
        public bool IsRegistered(Type type, string tag = null)
        {
            var key = (tag, type);
            if (_entriesMap.ContainsKey(key)) return true;
            if (_parentContainer != null) return _parentContainer.IsRegistered(type, tag);
            return false;
        }

        /// <summary>
        /// Разрешает зависимость по типу без generic-параметра (через рефлексию).
        /// Используется для Lazy<T>-обёртки.
        /// </summary>
        private object ResolveUntyped(Type type, string tag)
        {
            var method = typeof(DIContainer).GetMethod(nameof(Resolve), new[] { typeof(string) });
            var genericMethod = method.MakeGenericMethod(type);
            return genericMethod.Invoke(this, new object[] { tag });
        }

        /// <summary>
        /// Регистрирует объект, который будет автоматически дизпознут при выходе со сцены.
        /// Используется для gameplay-сервисов (WaveService, TowersService и т.д.)
        /// </summary>
        public void RegisterDisposableOnSceneExit(IDisposable disposable)
        {
            _sceneDisposables.Add(disposable);
        }

        /// <summary>
        /// Дизпозит все объекты, зарегистрированные через RegisterDisposableOnSceneExit,
        /// в порядке, обратном регистрации (сначала зависимые, потом базовые).
        /// </summary>
        public void DisposeSceneDisposables()
        {
            for (int i = _sceneDisposables.Count - 1; i >= 0; i--)
            {
                _sceneDisposables[i].Dispose();
            }
            _sceneDisposables.Clear();
        }

        public bool IsRegistered<T>(string tag = null)
        {
            var key = (tag, typeof(T));
            if (_entriesMap.ContainsKey(key)) return true;
            if (_parentContainer != null) return _parentContainer.IsRegistered<T>(tag);
            return false;
        }

        public void Dispose()
        {
            DisposeSceneDisposables();

            var entries = _entriesMap.Values;

            foreach (var entry in entries)
            {
                entry.Dispose();
            }
        }
    }
}
