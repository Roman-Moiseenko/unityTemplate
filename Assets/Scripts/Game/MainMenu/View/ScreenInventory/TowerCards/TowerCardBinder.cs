using System;
using R3;
using UnityEngine;

namespace Game.MainMenu.View.ScreenInventory.TowerCards
{
    public class TowerCardBinder : MonoBehaviour
    {

        private TowerCardViewModel _viewModel;
        private IDisposable _disposable;
        

        //TODO Отслеживать и перемещать модель в Binder 
        public ReactiveProperty<bool> IsInDeck = new(false);
        public void Bind(TowerCardViewModel viewModel)
        {
            var t = transform.GetComponent<RectTransform>(); //.position = new Vector3(10, 20, 0);
            
        //    Debug.Log(" " + t.localPosition + " " + t.sizeDelta + " " + t.anchoredPosition3D + " " + t.anchoredPosition);
            var d = Disposable.CreateBuilder();
          //  _viewModel = viewModel;
            t.anchoredPosition = viewModel.Position;
            _disposable = d.Build();
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}