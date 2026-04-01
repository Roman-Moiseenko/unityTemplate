
using System;
using System.Collections.Generic;
using System.Linq;
using Game.State.Inventory.HeroCards;
using Game.State.Inventory.SkillCards;
using Game.State.Inventory.TowerCards;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.State.Inventory.Deck
{
    public class DeckCard
    {
        public readonly DeckCardData Origin;
        public readonly ObservableList<int> TowerCardIds = new();
        public readonly ObservableList<int> SkillCardIds = new();
        public readonly ReactiveProperty<int> HeroCardId;

        public DeckCard(DeckCardData deckCardData)
        {
            Origin = deckCardData;
            HeroCardId = new ReactiveProperty<int>(deckCardData.HeroCardId);
            HeroCardId.Subscribe(newValue => deckCardData.HeroCardId = newValue);
            foreach (var towerCardId in deckCardData.TowerCardIds)
            {
                TowerCardIds.Add(towerCardId);
            }
            
            TowerCardIds.ObserveAdd().Subscribe(e =>
            {
                deckCardData.TowerCardIds.Add( e.Value);
            });

            TowerCardIds.ObserveRemove().Subscribe(e =>
            {
                deckCardData.TowerCardIds.Remove(e.Value);
            });

            foreach (var skillCardId in deckCardData.SkillCardIds)
            {
                SkillCardIds.Add(skillCardId);
            }

            SkillCardIds.ObserveAdd().Subscribe(e =>
            {
                deckCardData.SkillCardIds.Add(e.Value);
            });
            SkillCardIds.ObserveRemove().Subscribe(e =>
            {
                deckCardData.SkillCardIds.Remove(e.Value);
            });
        }

        public bool TowerCardInDeck(int uniqueId)
        {
            return TowerCardIds.Any(towerCardId => towerCardId == uniqueId);
        }

        public void ExtractTowerFromDeck(int uniqueId)
        {
            foreach (var towerCardId in TowerCardIds)
            {
                if (towerCardId != uniqueId) continue;
                TowerCardIds.Remove(towerCardId);
                return;
            }
        }

        public bool PushTowerToDeck(int uniqueId)
        {
            if (TowerCardIds.Count == 6) return false;
            
            TowerCardIds.Add(uniqueId);
            return true;
        }

        public bool SkillCardInDeck(int uniqueId)
        {
            return SkillCardIds.Any(skillCardId => skillCardId == uniqueId);
        }
        
        public void ExtractSkillFromDeck(int uniqueId)
        {
            foreach (var skillCardId in SkillCardIds)
            {
                if (skillCardId != uniqueId) continue;
                SkillCardIds.Remove(skillCardId);
                return;
            }
        }
        
        public bool PushSkillToDeck(int uniqueId)
        {
            var count = SkillCardIds.Count;
            if (SkillCardIds.Count == 2) return false;

            SkillCardIds.Add(uniqueId);
            return true;
        }
    }
}