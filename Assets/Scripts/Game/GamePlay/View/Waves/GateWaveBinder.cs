using R3;
using UnityEngine;

namespace Game.GamePlay.View.Waves
{
    public class GateWaveBinder : MonoBehaviour
    {
        private GateWaveViewModel _viewModel;
        [SerializeField] private Transform _gate;
        public void Bind(GateWaveViewModel viewModel)
        {
            _viewModel = viewModel;
            
            _viewModel.ShowGate.Subscribe(show =>
            {
                _gate.gameObject.SetActive(show);
            });
            _viewModel.Position.Subscribe(newPosition =>
            {
                transform.position =
                    new Vector3(_viewModel.Position.CurrentValue.x, 0, _viewModel.Position.CurrentValue.y);
                //TODO Смена позиций, перемещаем и поворачиваем модель ПЛАВНО
               // Debug.Log("_viewModel.Position " + _viewModel.Position.CurrentValue + " _viewModel.Direction " + _viewModel.Direction.CurrentValue);
            });
            _viewModel.Direction.Subscribe(newDirection =>
            {
                _gate.gameObject.transform.localEulerAngles = new Vector3(0, 90f * newDirection.y + 180f * newDirection.x,0);
            });

        }  
 
    }
}