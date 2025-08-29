
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
        public ObservableDictionary<int, int> TowerCardIds = new();
        public ObservableDictionary<int, int> SkillCardIds = new();
        public ReactiveProperty<int> HeroCardId;

        public DeckCard(DeckCardData deckCardData)
        {
            Origin = deckCardData;
            HeroCardId = new ReactiveProperty<int>(deckCardData.HeroCardId);
            HeroCardId.Subscribe(newValue => deckCardData.HeroCardId = newValue);
            foreach (var value in deckCardData.TowerCardIds)
            {
                TowerCardIds.Add(value.Key, value.Value);
            }
            
            TowerCardIds.ObserveAdd().Subscribe(newValue =>
            {
                deckCardData.TowerCardIds.Add(newValue.Value.Key, newValue.Value.Value);
            });

            TowerCardIds.ObserveRemove().Subscribe(v =>
            {
                deckCardData.TowerCardIds.Remove(v.Value.Key);
            });
       /*     
            TowerCardIds.ObserveChanged().Subscribe(e =>
            {
                deckCardData.TowerCardIds[e.NewItem.Key] = e.NewItem.Value;
            });
            */
            foreach (var value in deckCardData.SkillCardIds)
            {
                SkillCardIds.Add(value.Key, value.Value);
            }
        }

        public bool TowerCardInDeck(int uniqueId)
        {
            return TowerCardIds.Any(towerCardId => towerCardId.Value == uniqueId);
        }

        public void ExtractFromDeck(int uniqueId)
        {
            foreach (var towerCardId in TowerCardIds)
            {
                if (towerCardId.Value != uniqueId) continue;
                TowerCardIds.Remove(towerCardId.Key);
                return;
            }
        }

        public int PushToDeck(int uniqueId)
        {
            var count = TowerCardIds.Count;
            if (count == 6)
            {
                ExtractFromDeck(TowerCardIds[1]);
                return 1;
            }
            else
            {
                for (var i = 1; i <= 6; i++)
                {
                    if (TowerCardIds.TryGetValue(i, out var value)) continue;
                    TowerCardIds.Add(i, uniqueId);
                    return i;
                }
            }

            throw new Exception("Ошибка добавления в колоду");
        }
    }
}