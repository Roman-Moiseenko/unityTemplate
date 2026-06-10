# 15. StorageManager — кэширование данных

## Назначение

`StorageManager` — система кэширования данных (преимущественно текстур и спрайтов) для оптимизации повторных загрузок. Позволяет загружать ресурсы по ключу и получать их с кэша при повторном запросе.

## Интерфейс

```csharp
public class StorageManager : IDisposable
{
    // Загрузка текстуры из Resources
    public Texture2D GetTexture(string path);
    
    // Загрузка спрайта из Resources
    public Sprite GetSprite(string path);
    
    // Загрузка и кэширование
    public T GetResource<T>(string path) where T : UnityEngine.Object;
    
    // Очистка кэша
    public void ClearCache();
    
    // Выгрузка неиспользуемых ресурсов
    public void UnloadUnusedAssets();
}
```

## Принцип работы

1. При запросе ресурса проверяется кэш (`Dictionary<string, object>`)
2. Если ресурс есть в кэше — возвращается сразу
3. Если нет — загружается из Resources, сохраняется в кэш и возвращается
4. При выходе со сцены или при необходимости можно очистить кэш

```csharp
public T GetResource<T>(string path) where T : UnityEngine.Object
{
    if (_cache.TryGetValue(path, out var cached))
        return cached as T;
    
    var resource = Resources.Load<T>(path);
    if (resource != null)
        _cache[path] = resource;
    
    return resource;
}
```

## Регистрация

```csharp
// В корневом контейнере GameEntryPoint
var storageManager = new StorageManager();
container.RegisterInstance<StorageManager>(storageManager);
```

## Использование

```csharp
public class SomeService
{
    private readonly StorageManager _storageManager;
    
    public SomeService(StorageManager storageManager)
    {
        _storageManager = storageManager;
    }
    
    public void LoadTowerSprite(string configId)
    {
        var sprite = _storageManager.GetSprite($"Images/Towers/{configId}");
        // Использование спрайта
    }
}
```

## Связь с ImageManager

`StorageManager` работает на более низком уровне — загружает ресурсы из папки Resources. `ImageManagerViewModel` в свою очередь может использовать `StorageManager` для загрузки спрайтов, добавляя поверх маппинг ID → путь.

---

*Читайте далее: [16_ObjectPooling.md](16_ObjectPooling.md)*
