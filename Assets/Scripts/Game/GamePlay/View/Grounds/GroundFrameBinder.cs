﻿using System;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Grounds
{
    public class GroundFrameBinder : MonoBehaviour
    {
        [SerializeField] private Material allowed;
        [SerializeField] private Material forbidden;
        
        [SerializeField] private GameObject frame;
        private IDisposable _disposable;

        
        public void Bind(GroundFrameViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            transform.localPosition = new Vector3(
                viewModel.GetPosition().x,
                0,
                viewModel.GetPosition().y);

            viewModel.Enabled.Subscribe(newValue =>
            {
                if (newValue)
                {
                    frame.GetComponent<MeshRenderer>().material = allowed;
                } 
                else
                {
                    frame.GetComponent<MeshRenderer>().material = forbidden;
                }
            }).AddTo(ref d);
            _disposable = d.Build();
        }
        
        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}