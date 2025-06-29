
using Game.State.Inventory.HeroCards;
using Game.State.Inventory.SkillCards;
using Game.State.Inventory.TowerCards;
using ObservableCollections;
using R3;

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

            TowerCardIds.ObserveChanged().Subscribe(e =>
            {
                deckCardData.TowerCardIds[e.NewItem.Key] = e.NewItem.Value;
            });
            
            foreach (var value in deckCardData.SkillCardIds)
            {
                SkillCardIds.Add(value.Key, value.Value);
            }
            
            
        }
    }
}