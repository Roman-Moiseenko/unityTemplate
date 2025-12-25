using System;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames
{
    public class FrameBlockBinder : MonoBehaviour
    {
        private FrameBlockViewModel _viewModel;
        
        [SerializeField] private Material allowed;
        [SerializeField] private Material allowedSelected;
        [SerializeField] private Material forbidden;
        [SerializeField] private Material forbiddenSelected;
        [SerializeField] private GameObject frame;
        [SerializeField] public Transform Element;
        [SerializeField] private ParticleSystem cloud;
        private Vector3 _targetPosition;
        private Vector3 _targetPositionElement;
        private bool _isMoving = false;
        private const int speed = 20;
        private const float smoothTime = 0.2f;
        private Vector3 _velocity;
        private IDisposable _disposable;

        private bool downElement = false;
        private bool showCloudDust = false;
        
        public void Bind(FrameBlockViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            var meshRenderer = frame.GetComponent<MeshRenderer>();
            var matBlock = new MaterialPropertyBlock();
            
            Observable.Merge(viewModel.Enable, viewModel.IsSelected).Subscribe(v =>
            {
                //SetMaterial();
                
                meshRenderer.GetPropertyBlock(matBlock);
                matBlock.SetInt("_Enabled", viewModel.Enable.CurrentValue ? 1 : 0);
                matBlock.SetInt("_Selected", viewModel.IsSelected.CurrentValue ? 1 : 0);
                meshRenderer.SetPropertyBlock(matBlock);
                
            }).AddTo(ref d);
            viewModel.Position.Subscribe(newPosition =>
            {
                _targetPosition = new Vector3(newPosition.x, transform.position.y, newPosition.y);
                _isMoving = true;
            }).AddTo(ref d);


            viewModel.Rotate
                .Subscribe(newValue => transform.localEulerAngles = new Vector3(0, 90f * newValue,0))
                .AddTo(ref d);
            
            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x,
                transform.position.y,
                viewModel.Position.CurrentValue.y
            );

            viewModel.StartRemoveFlag.Where(x => x).Subscribe(_ =>
            {
                frame.SetActive(false);
                downElement = true;
                _targetPositionElement = Vector3.zero;
            }).AddTo(ref d);
            
            _disposable = d.Build();
        }
        
        private void Update()
        {
            if (_isMoving)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, smoothTime, speed );
                if (_velocity.magnitude < 0.0005)
                {
                    _isMoving = false;
                    transform.position = _targetPosition;
                }
            }

            if (downElement) //Опускаем Элементы
            {
                Element.localPosition = Vector3.Lerp(Element.localPosition, _targetPositionElement, 0.15f);
                if (Element.localPosition.y < 0.01)
                {
                    downElement = false;
                    showCloudDust = true; //Запуск пыли
                    cloud.Play();
                }
            }
            if (showCloudDust && !cloud.isPlaying)
            {
                showCloudDust = false;
                _viewModel.FinishRemoveFlag.OnNext(true);
            }
        }
        
        private void SetMaterial()
        {
            var _e = _viewModel.Enable.CurrentValue;
            var _s = _viewModel.IsSelected.CurrentValue;
            if (_e && _s)
            {
                frame.GetComponent<MeshRenderer>().material = allowedSelected; 
            }
            if (_e && !_s)
            {
                frame.GetComponent<MeshRenderer>().material = allowed; 
            }
            if (!_e && _s)
            {
                frame.GetComponent<MeshRenderer>().material = forbiddenSelected; 
            }
            if (!_e && !_s)
            {
                frame.GetComponent<MeshRenderer>().material = forbidden; 
            }
        }
        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}