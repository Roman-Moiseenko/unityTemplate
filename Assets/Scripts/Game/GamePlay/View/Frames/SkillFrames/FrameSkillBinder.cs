using System;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames.SkillFrames
{
    public class FrameSkillBinder : MonoBehaviour
    {
        [SerializeField] private Transform container;

        private FrameSkillModelBinder skillModelBinder;
        private IDisposable _disposable;

        public void Bind(FrameSkillViewModel viewModel)
        {
            //TODO сделать загрузку модели по viewModel.ConfigId и запуск Эффекта
            CreateSkill(viewModel);
            var d = Disposable.CreateBuilder();
            viewModel.Position.Subscribe(p =>
            {
//                Debug.Log("Position " + p);
                transform.position = new Vector3(p.x, 0, p.y);
            }).AddTo(ref d);
            viewModel.Enable.Subscribe(v => gameObject.SetActive(v));
            _disposable = d.Build();
        }

        private void CreateSkill(FrameSkillViewModel viewModel)
        {
            var configId = viewModel.ConfigId;
            
            var prefabPath = $"Prefabs/Gameplay/Skills/{configId}/Frame{configId}";
            var skillPrefab = Resources.Load<FrameSkillModelBinder>(prefabPath);
            skillModelBinder = Instantiate(skillPrefab, container);
            skillModelBinder.Bind(viewModel);


        }
        private void OnDestroy()
        {
            _disposable?.Dispose();
            
            Destroy(skillModelBinder.gameObject);
            Destroy(skillModelBinder);
        }
    }
}