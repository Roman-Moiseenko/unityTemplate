using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames
{
    public class FrameSkillBinder : MonoBehaviour
    {
        [SerializeField] private Transform container;


        public void Bind(FrameSkillViewModel viewModel)
        {
            //TODO сделать загрузку модели по viewModel.ConfigId и запуск Эффекта

            viewModel.Position.Subscribe(p =>
            {
//                Debug.Log("Position " + p);
                transform.position = new Vector3(p.x, 0, p.y);
            });
            viewModel.Enable.Subscribe(v => gameObject.SetActive(v));
        }
    }
}