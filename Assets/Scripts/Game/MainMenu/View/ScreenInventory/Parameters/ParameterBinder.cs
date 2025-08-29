
using System;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenInventory.Parameters
{
    public class ParameterBinder : MonoBehaviour
    {
        [SerializeField] private Image imageParameter;
        [SerializeField] private TMP_Text textName;
        [SerializeField] private TMP_Text textValue;
        [SerializeField] private TMP_Text textUpgrade;
        private IDisposable _disposable;

        public void Bind(Sprite imageParam, string nameParam, string valueParam, string measure, ReadOnlyReactiveProperty<float> value)
        {
            var d = Disposable.CreateBuilder();
            
            imageParameter.sprite = imageParam;
            textName.text = nameParam;
            textValue.text = valueParam + " " + measure;
            value.Subscribe(v => textValue.text = v + " " + measure).AddTo(ref d);
            gameObject.SetActive(true);

            _disposable = d.Build();
        }
        
        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}