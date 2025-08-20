using System;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Waves
{
    public class GateWaveBinder : MonoBehaviour
    {
        private GateWaveViewModel _viewModel;
        [SerializeField] private Transform _gate;

        private Animator _animator;
        private IDisposable _disposable;

        public void Bind(GateWaveViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _gate.gameObject.SetActive(false);
            _viewModel = viewModel;
            _animator = gameObject.GetComponent<Animator>();
            _viewModel.ShowGateWave.Skip(1).Subscribe(showGate =>
            {
                if (showGate)
                {
                    _gate.gameObject.SetActive(true);
                    _animator.Play("gate_wave_start");
                }
                else
                {
                    _animator.Play("gate_wave_finish");
                }
            }).AddTo(ref d);
            
            _viewModel.Position.Subscribe(newPosition =>
            {
                transform.position =
                    new Vector3(_viewModel.Position.CurrentValue.x, 0, _viewModel.Position.CurrentValue.y);
            }).AddTo(ref d);
            _viewModel.Direction.Subscribe(newDirection =>
            {
                _gate.gameObject.transform.localEulerAngles =
                    new Vector3(0, 90f * newDirection.y + 180f * newDirection.x, 0);
            }).AddTo(ref d);
            _disposable = d.Build();
        }

        public void EndFinishAnimation()
        {
            _gate.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}