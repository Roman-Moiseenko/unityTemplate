using System;
using System.Collections.Generic;
using System.Linq;
using Game.MainMenu.Commands.SkillCommands;
using Game.MainMenu.Commands.TowerCommands;
using Game.Settings;
using Game.State.Inventory;
using Game.State.Inventory.Deck;
using Game.State.Inventory.SkillCards;
using Game.State.Inventory.TowerCards;
using Game.State.Root;
using MVVM.CMD;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.MainMenu.Commands.InventoryCommands
{
    public class CommandCreateInventoryHandler : ICommandHandler<CommandCreateInventory>
    {
        private readonly GameStateProxy _gameState;
        private readonly GameSettings _gameSettings;
        private readonly ICommandProcessor _cmd;

        public CommandCreateInventoryHandler(
            GameStateProxy gameState,
            GameSettings gameSettings,
            ICommandProcessor cmd)
        {
            _gameState = gameState;
            _gameSettings = gameSettings;
            _cmd = cmd;
        }

        public bool Handle(CommandCreateInventory commandCreate)
        {
            var towerCards = _gameSettings.InventoryInitialSettings.TowerCards;
            var towerPlans = _gameSettings.InventoryInitialSettings.TowerPlans;
            var configTowers = _gameSettings.TowersSettings.AllTowers;

            //            Debug.Log(JsonConvert.SerializeObject(_gameSettings.InventoryInitialSettings, Formatting.Indented));

            foreach (var towerPlan in towerPlans)
            {
                var commandTowerPlan = new CommandTowerPlanAdd
                {
                    ConfigId = towerPlan.ConfigId,
                    Amount = towerPlan.Amount
                };
                _cmd.Process(commandTowerPlan, false);
            }

            foreach (var towerCard in towerCards) //Начальные башни из настроек
            {
                var configTower = configTowers.FirstOrDefault(t => t.ConfigId == towerCard.ConfigId);
                if (configTower == null) throw new Exception($"towerConfig = {towerCard.ConfigId}  Not Find");
                var commandTowerCard = new CommandTowerCardAdd
                {
                    ConfigId = towerCard.ConfigId,
                    EpicLevel = towerCard.epicCardLevel,
                    Level = towerCard.Level
                };
                _cmd.Process(commandTowerCard, false);
            }


            //Начальные навыки из настроек
            var skillCards = _gameSettings.InventoryInitialSettings.SkillCards;
            var skillPlans = _gameSettings.InventoryInitialSettings.SkillPlans;
            var configSkills = _gameSettings.SkillsSettings.AllSkills;
            foreach (var skillPlan in skillPlans)
            {
                var commandSkillPlan = new CommandSkillPlanAdd()
                {
                    ConfigId = skillPlan.ConfigId,
                    Amount = skillPlan.Amount
                };
                _cmd.Process(commandSkillPlan, false);
            }

            foreach (var skillCard in skillCards) //Начальные башни из настроек
            {
                var configSkill = configSkills.FirstOrDefault(t => t.ConfigId == skillCard.ConfigId);
                if (configSkill == null) throw new Exception($"skillConfig = {skillCard.ConfigId}  Not Find");
                var commandSkillCard = new CommandSkillCardAdd
                {
                    ConfigId = skillCard.ConfigId,
                    EpicLevel = skillCard.epicCardLevel,
                    Level = skillCard.Level
                };
                _cmd.Process(commandSkillCard, false);
            }

            var initialDeck = new DeckCardData(); //Создаем начальную колоду

            foreach (var inventoryItem in _gameState.Inventory.Items)
            {
                if (inventoryItem is TowerCard towerCard && initialDeck.TowerCardIds.Count < 6)
                    initialDeck.TowerCardIds.Add(towerCard.UniqueId); //Добавляем начальные башни в колоду    

                if (inventoryItem is SkillCard skillCard && initialDeck.SkillCardIds.Count < 3)
                    initialDeck.SkillCardIds.Add(skillCard.UniqueId); //Добавляем начальные навыки в колоду    
            }

            _gameState.Inventory.DeckCards.Add(1, new DeckCard(initialDeck));


            //TODO Начальный герой из настроек


            _gameState.HardCurrency.OnNext(5000);
            _gameState.SoftCurrency.OnNext(45000);
            return true;
        }
    }
}