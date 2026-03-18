using System;
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

        private const int Delta = 19;
        public ReactiveProperty<bool> Value;
        public void Bind(bool value)
        {
            Value = new ReactiveProperty<bool>(value);

            ToggleUI(value ? 1 : 0, value);
//if (value)
      //      {
     //           var pos = handler.position;
      //          pos.x += Delta * 2;
      //          handler.position = pos;
      //      }
      //      backOff.gameObject.SetActive(!value);
       //     backOn.gameObject.SetActive(value);
            
            Value.Skip(1).Subscribe(x =>
            {
               // var pos = handler.position;
                var direction = x ? 1 : -1;
                ToggleUI(direction, x);
              //  pos.x += direction * Delta * 2;
              //  handler.position = pos;
              //  backOff.gameObject.SetActive(!x);
              //  backOn.gameObject.SetActive(x);
            });
        }

        private void ToggleUI(int direction, bool on)
        {
            var pos = handler.position;
            pos.x += direction * Delta * 2;
            handler.position = pos;
            backOff.gameObject.SetActive(!on);
            backOn.gameObject.SetActive(on);
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