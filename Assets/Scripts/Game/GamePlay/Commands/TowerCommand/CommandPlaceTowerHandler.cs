using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Maps.Towers;
using Game.State.Root;
using MVVM.CMD;

namespace Game.GamePlay.Commands.TowerCommand
{
    public class CommandPlaceTowerHandler : ICommandHandler<CommandPlaceTower>
    {
        private readonly GameplayStateProxy _gameplayState;
        private readonly TowersSettings _towersSettings;
        private readonly ICommandProcessor _cmd;

        public CommandPlaceTowerHandler(GameplayStateProxy gameplayState,
            TowersSettings towersSettings, ICommandProcessor cmd)
        {
            _gameplayState = gameplayState;
            _towersSettings = towersSettings;
            _cmd = cmd;
        }
        
        public bool Handle(CommandPlaceTower command)
        {
            var entityId = _gameplayState.CreateEntityID(); //Получаем уникальный ID
            var towerSettings = _towersSettings.AllTowers.Find(t => t.ConfigId == command.ConfigId);
            
            var newTowerEntity = new TowerEntityData() //Создаем сущность игрового объекта
            {
                UniqueId = entityId,
                Position = command.Position,
                ConfigId = command.ConfigId,
                TypeEnemy = towerSettings.TypeEnemy,
                IsMultiShot = towerSettings.MultiShot,
                IsSingleTarget = towerSettings.Shot.Single,
                SpeedShot = towerSettings.Shot.Speed,
                IsOnRoad = towerSettings.OnRoad,
                Defence = towerSettings.Defence,
                IsPlacement = towerSettings.Placement,
                Placement = command.Placement
            };
            var newTower = new TowerEntity(newTowerEntity); //Оборачиваем его Прокси
            _gameplayState.Towers.Add(newTower);//Добавляем в список объектов карты
            return true;
        }
    }
}
