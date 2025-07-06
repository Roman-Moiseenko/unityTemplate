using R3;
using UnityEngine;

namespace Game.GamePlay.View.Waves
{
    public class GateWaveBinder : MonoBehaviour
    {
        private GateWaveViewModel _viewModel;
        public void Binder(GateWaveViewModel viewModel)
        {
            _viewModel = viewModel;
            
            _viewModel.ShowInfo.Subscribe(show =>
            {
                if (show)
                {
                    //TODO Показываем инфо блок
                }
                else
                {
                    //TODO Показываем ворота
                }
            });
            _viewModel.Position.Subscribe(newPosition =>
            {
                //TODO Смена позиций, перемещаем и поворачиваем модель ПЛАВНО
                Debug.Log("_viewModel.Position " + _viewModel.Position.CurrentValue + " _viewModel.Direction " + _viewModel.Direction.CurrentValue);
            });
        }


        
        
    }
}