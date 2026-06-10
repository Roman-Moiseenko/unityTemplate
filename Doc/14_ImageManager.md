# 14. ImageManager — система изображений

## Назначение

`ImageManager` — система управления изображениями (спрайтами) в проекте. Позволяет загружать, кэшировать и отображать изображения по строковому ID.

## ImageManagerBinder

`ImageManagerBinder` — MonoBehaviour, который загружается на сцену на этапе инициализации корневого контейнера. Регистрируется как синглтон на всё время жизни приложения.

```csharp
public class ImageManagerBinder : MonoBehaviour
{
    // Загружается из Resources/"ImageManager"
    // Содержит ImageManagerViewModel
    // Привязывает ImageManager к визуальным компонентам
}
```

## ImageManagerViewModel

```csharp
public class ImageManagerViewModel : IDisposable
{
    // Загрузка изображения по ID
    public Observable<Sprite> LoadImage(string imageId);
    
    // Кэш загруженных изображений
    public Dictionary<string, Sprite> Cache;
    
    // Маппинг ID → путь в Resources
    public Dictionary<string, string> Mapping;
}
```

## Принцип работы

1. Каждый спрайт имеет строковый ID (например, `"tower_01"`, `"mob_goblin"`, `"skill_fireball"`)
2. `ImageManagerViewModel.LoadImage(imageId)` проверяет кэш
3. Если спрайт уже загружен — возвращает из кэша
4. Если нет — загружает из Resources, сохраняет в кэш и возвращает
5. UI-элементы подписываются через R3 и обновляют `Image.sprite`

## Использование в Binder

```csharp
public class TowerBinder : MonoBehaviour
{
    [SerializeField] private Image _towerImage;
    
    public void Bind(TowerViewModel viewModel, ImageManagerViewModel imageManager)
    {
        // Подписка на изменение спрайта башни
        viewModel.SpriteId.Subscribe(spriteId => {
            imageManager.LoadImage(spriteId).Subscribe(sprite => {
                _towerImage.sprite = sprite;
            });
        });
    }
}
```

## Структура спрайтов

```
Resources/
└── Images/
    ├── Towers/
    │   ├── tower_archer_01
    │   ├── tower_magic_01
    │   └── ...
    ├── Mobs/
    │   ├── mob_goblin_01
    │   ├── mob_ogre_01
    │   └── ...
    ├── Skills/
    ├── Icons/
    ├── UI/
    └── Effects/
```

## Регистрация

```csharp
// В корневом контейнере GameEntryPoint
var imageManagerPrefab = Resources.Load<GameObject>("ImageManager");
var imageManagerInstance = Instantiate(imageManagerPrefab);
var imageManagerViewModel = imageManagerInstance.GetComponent<ImageManagerBinder>().ViewModel;

container.RegisterInstance<ImageManagerViewModel>(imageManagerViewModel);
```

---

*Читайте далее: [15_StorageManager.md](15_StorageManager.md)*
