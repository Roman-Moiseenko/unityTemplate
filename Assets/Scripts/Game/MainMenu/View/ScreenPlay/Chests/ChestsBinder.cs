using System.Collections.Generic;
using Game.State.Root;
using UnityEngine;

namespace Game.MainMenu.View.ScreenPlay.Chests
{
    public class ChestsBinder : MonoBehaviour
    {
        [SerializeField] private List<CellChestBinder> cells;
        
        public void Bind(ChestsViewModel viewModel)
        {
            //Проходим все модели ячеек и запускаем Байнды
            for (int i = 1; i <= GameStateProxy.MaxChest; i++)
            {
                cells[i - 1].Bind(viewModel.CellsViewModel[i]);
            }
        }
    }
}