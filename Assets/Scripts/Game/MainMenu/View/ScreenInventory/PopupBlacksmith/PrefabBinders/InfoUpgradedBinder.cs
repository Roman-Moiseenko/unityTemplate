using System.Collections.Generic;
using Game.State.Inventory;
using TMPro;
using UnityEngine;

namespace Game.MainMenu.View.ScreenInventory.PopupBlacksmith.PrefabBinders
{
    public class InfoUpgradedBinder : MonoBehaviour
    {

        [SerializeField] private Transform infoEmptyBlock;
        [SerializeField] private Transform infoTowerBlock;
        [SerializeField] private Transform useBlock;
        [SerializeField] private List<Transform> parameters;
        
        public void Bind(InfoUpgradedViewModel viewModel)
        {
            foreach (var parameter in parameters)
            {
                parameter.gameObject.SetActive(false);
            }
            
            infoEmptyBlock.gameObject.SetActive(false);
            infoTowerBlock.gameObject.SetActive(true);
            useBlock.gameObject.SetActive(true);
            infoTowerBlock.Find("textCardTitle").GetComponent<TMP_Text>().text = viewModel.NameTower;
            
            var index = 0;
            foreach (var parameter in viewModel.Parameters)
            {
                parameters[index].GetComponent<InfoParameterBinder>().Bind(
                    parameter);
                index++;
            }

            infoTowerBlock.GetComponent<RectTransform>().sizeDelta = new Vector2(
                720, index * 40 + 50);
            useBlock.GetComponent<TMP_Text>().text = $"2 x {viewModel.NameEpic} {viewModel.NameTower}";
        }

        public void Empty()
        {
            infoEmptyBlock.gameObject.SetActive(true);
            infoTowerBlock.gameObject.SetActive(false);
            useBlock.gameObject.SetActive(false);
        }
    }
}