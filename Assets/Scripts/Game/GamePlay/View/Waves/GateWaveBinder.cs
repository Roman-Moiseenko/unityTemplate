using R3;
using UnityEngine;

namespace Game.GamePlay.View.Waves
{
    public class GateWaveBinder : MonoBehaviour
    {
        private GateWaveViewModel _viewModel;
        [SerializeField] private Transform _gate;

        private Animator _animator;

        public void Bind(GateWaveViewModel viewModel)
        {
            _gate.gameObject.SetActive(false);
            _viewModel = viewModel;
            _animator = gameObject.GetComponent<Animator>();
            _viewModel.ShowGateWave.Skip(1).Subscribe(show =>
            {
                if (show)
                {
                    _gate.gameObject.SetActive(true);
                    _animator.Play("gate_wave_start");
                }
                else
                {
                    _animator.Play("gate_wave_finish");
                }
            });
            
            _viewModel.Position.Subscribe(newPosition =>
            {
                transform.position =
                    new Vector3(_viewModel.Position.CurrentValue.x, 0, _viewModel.Position.CurrentValue.y);
            });
            _viewModel.Direction.Subscribe(newDirection =>
            {
                _gate.gameObject.transform.localEulerAngles =
                    new Vector3(0, 90f * newDirection.y + 180f * newDirection.x, 0);
            });
        }

        public void EndFinishAnimation()
        {
            _gate.gameObject.SetActive(false);
        }
    }
}