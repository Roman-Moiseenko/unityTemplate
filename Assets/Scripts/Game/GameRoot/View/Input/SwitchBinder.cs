using System;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameRoot.View.Input
{
    public class SwitchBinder : MonoBehaviour
    {
        [SerializeField] private Button switchButton;
        [SerializeField] private Transform backOff;
        [SerializeField] private Transform backOn;
        [SerializeField] private Transform handler;

        private const int DELTA = 38;
        public ReactiveProperty<bool> Value;
        public void Bind(bool value)
        {
            Value = new ReactiveProperty<bool>(value);
            //Базовое включение
            var pos = handler.position;
            pos.x += (value ? 1 : 0) * DELTA * 2;
            handler.position = pos;
            SetActiveBacks(value);
            Value.Skip(1).Subscribe(x =>
            {
                var direction = x ? 1 : -1;
                handler
                    .DOLocalMoveX(handler.localPosition.x + direction * DELTA * 2, 0.3f)
                    .From(handler.localPosition.x)
                    .SetUpdate(true);
                SetActiveBacks(x);
            });
        }

        private void SetActiveBacks(bool value)
        {
            backOff.gameObject.SetActive(!value);
            backOn.gameObject.SetActive(value);
        }
        
        private void OnEnable()
        {
            switchButton.onClick.AddListener(OnClickToggle);
        }
        private void OnDisable()
        {
            switchButton.onClick.RemoveListener(OnClickToggle);
        }

        private void OnClickToggle()
        {
            Value.Value = !Value.Value;
        }

        public bool Toggle()
        {
            Value.Value = !Value.Value;
            return Value.CurrentValue;
        }
    }
}