using System.Collections.Generic;
using UnityEngine;

namespace Game.MainMenu.View.ScreenInventory.Deck
{
    public class DeckBinder : MonoBehaviour
    {
        [SerializeField] private List<Transform> towerCards = new(6);
        [SerializeField] private List<Transform> skillCards = new(2);
        [SerializeField] private Transform heroCard;


        public void Bind()
        {
            
        }
    }
}