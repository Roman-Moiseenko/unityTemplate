using System;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.VFX;

namespace Game.GamePlay.View.Waves
{
    public class GateWaveBinder : MonoBehaviour
    {
        private GateWaveViewModel _viewModel;
        [SerializeField] private Transform _gate;
        [SerializeField] private VisualEffect portal;

        private Animator _animator;
        private IDisposable _disposable;
        private float _targetRadius;
        private float _currentRadius;
        private float _speed;
        private bool _startAnimation = false;

        private void Awake()
        {
            portal.Stop();
        }

        public void Bind(GateWaveViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            //_gate.gameObject.SetActive(false);
            _viewModel = viewModel;
            
            _animator = gameObject.GetComponent<Animator>();
            _viewModel.ShowGateWave.Skip(1).Subscribe(showGate =>
            {
                if (showGate)
                {
                    //  _gate.gameObject.SetActive(true);
                    _currentRadius = 0f;
                    portal.SetFloat("Radius", _currentRadius);
                    portal.Play();
                    portal.gameObject.SetActive(true);
                    _targetRadius = 0.5f;
                    // _speed = 0.01f;
                    //_animator.Play("gate_wave_start");
                }
                else
                {
                    _currentRadius = 0.5f;
                    // portal.SetFloat("Radius", 0.5f);
                    _targetRadius = 0f;
                    //_speed = -0.01f;
                    //_animator.Play("gate_wave_finish");
                }

                _startAnimation = true;
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


        private void Update()
        {
            if (!_startAnimation) return;

            _currentRadius = Mathf.Lerp(_currentRadius, _targetRadius, Time.unscaledDeltaTime * 10f);
            portal.SetFloat("Radius", _currentRadius);
            
            if (Mathf.Abs(_currentRadius - _targetRadius) < 0.01)
            {
                _startAnimation = false;
                if (_targetRadius == 0f)
                {
                    portal.Stop();
                    portal.gameObject.SetActive(false);
                }
            }
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