using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.MainMenu.View.ScreenInventory.PopupBlacksmith.PrefabBinders
{
    public class InfoParameterBinder : MonoBehaviour
    {
        [SerializeField] private TMP_Text textParameter;
        [SerializeField] private TMP_Text textValue;
        [SerializeField] private TMP_Text textValueAfter;

        public void Bind(KeyValuePair<string, Vector2> parameter)
        {
            textParameter.text = parameter.Key;
            textValue.text = parameter.Value.x.ToString();
            textValueAfter.text = parameter.Value.y.ToString();
            transform.gameObject.SetActive(true);
        }
    }
}