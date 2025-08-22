using System;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.MainScreen.BotomMenu
{
    public class ButtonBinder : MonoBehaviour
    {
        private bool _clicked = false;

      //  [SerializeField] private string nameButton;
        private Animator _animator;
        private Button _button;
        public void Bind()
        {
            _button = transform.GetComponent<Button>();
            _animator = transform.GetComponent<Animator>();
        }

        public void Click()
        {
            if (_clicked) return;
            _clicked = true;
            _animator.Play("button_select");
        }

        public void UnClick()
        {
            if (!_clicked) return;
            _clicked = false;
            _animator.Play("button_unselect");
        }        
        
        public void Resize(Vector2 sizeDelta, Vector3 position)
        {
            transform.GetComponent<RectTransform>().sizeDelta = sizeDelta;
            transform.GetComponent<RectTransform>().position = position;
        }

        public bool HasName(string nameField)
        {
            return name == nameField;
        }


    }
}