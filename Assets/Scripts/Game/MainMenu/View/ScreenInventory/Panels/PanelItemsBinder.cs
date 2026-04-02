using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenInventory.Panels
{
    
    public class PanelItemsBinder : PanelBinder
    {
        [SerializeField] private Button btnSort;
        [SerializeField] private Button btnBlacksmith;
        [SerializeField] private Transform containerCards;
        [SerializeField] private Transform containerPlans;

        
        public void Bind(ScreenInventoryViewModel viewModel)
        {
            
        }
    }
}