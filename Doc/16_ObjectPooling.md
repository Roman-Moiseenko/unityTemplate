# 16. Object Pooling (Storage/PoolMono)

## Назначение

Object Pooling используется для оптимизации создания и удаления часто используемых игровых объектов: мобов, выстрелов, эффектов, воинов. Вместо Instantiate/Destroy пул переиспользует экземпляры.

## PoolMono

```csharp
public class PoolMono<T> where T : MonoBehaviour
{
    private readonly Stack<T> _pool = new();
    private readonly Func<T> _factory;
    private readonly Transform _parent;
    
    public PoolMono(Func<T> factory, int preloadCount = 0, Transform parent = null)
    {
        _factory = factory;
        _parent = parent;
        
        // Предзагрузка
        for (int i = 0; i < preloadCount; i++)
        {
            var obj = CreateNew();
            obj.gameObject.SetActive(false);
            _pool.Push(obj);
        }
    }
    
    // Взять объект из пула (или создать новый)
    public T Get()
    {
        if (_pool.Count > 0)
        {
            var obj = _pool.Pop();
            obj.gameObject.SetActive(true);
            return obj;
        }
        
        var newObj = CreateNew();
        newObj.gameObject.SetActive(true);
        return newObj;
    }
    
    // Вернуть объект в пул
    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(_parent);
        _pool.Push(obj);
    }
    
    // Создать новый объект
    private T CreateNew()
    {
        var obj = _factory();
        obj.transform.SetParent(_parent);
        return obj;
    }
    
    // Очистить пул
    public void Clear()
    {
        foreach (var obj in _pool)
            GameObject.Destroy(obj.gameObject);
        _pool.Clear();
    }
}
```

## Использование в проекте

### Пул мобов

```csharp
public class MobPool
{
    private readonly PoolMono<MobBinder> _pool;
    
    public MobPool(MobBinder prefab, Transform parent)
    {
        _pool = new PoolMono<MobBinder>(
            factory: () => GameObject.Instantiate(prefab, parent),
            preloadCount: 10  // Предзагрузить 10 мобов
        );
    }
    
    public MobBinder GetMob()
    {
        return _pool.Get();
    }
    
    public void ReturnMob(MobBinder mob)
    {
        _pool.Return(mob);
    }
}
```

### Пул выстрелов

```csharp
public class ShotPool
{
    private readonly PoolMono<ShotBinder> _pool;
    
    public ShotPool(ShotBinder prefab, Transform parent)
    {
        _pool = new PoolMono<ShotBinder>(
            factory: () => GameObject.Instantiate(prefab, parent),
            preloadCount: 20
        );
    }
    
    public ShotBinder GetShot() => _pool.Get();
    public void ReturnShot(ShotBinder shot) => _pool.Return(shot);
}
```

## StorageManager — в контексте пулинга

Помимо кэша ресурсов, StorageManager может выступать как фабрика префабов:

```csharp
public class StorageManager
{
    public T GetPrefab<T>(string path) where T : MonoBehaviour
    {
        return Resources.Load<T>(path);
    }
}
```

## Преимущества

- Уменьшение количества операций Instantiate/Destroy (снижение GC)
- Переиспользование уже выделенной памяти
- Контроль максимального количества объектов
- Предзагрузка объектов до момента использования

---

*Читайте далее: [17_Inventory.md](17_Inventory.md)*
