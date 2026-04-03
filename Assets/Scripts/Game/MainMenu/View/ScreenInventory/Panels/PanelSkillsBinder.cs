using System;
using System.Collections.Generic;
using Game.MainMenu.View.Common;
using Game.MainMenu.View.ScreenInventory.SkillCards;
using Game.MainMenu.View.ScreenInventory.SkillPlans;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenInventory.Panels
{
    public class PanelSkillsBinder : PanelBinder
    {
        [SerializeField] private Button btnSort;
        [SerializeField] private Button btnBlacksmith;
        [SerializeField] private Transform containerCards;
        [SerializeField] private Transform containerPlans;
        private readonly Dictionary<int, SkillCardBinder> _createdSkillCardMap = new();
        private readonly Dictionary<int, SkillPlanBinder> _createdSkillPlanMap = new();
        private IDisposable _disposable;
        private ScreenInventoryViewModel _viewModel;

        public void Bind(ScreenInventoryViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            foreach (var skillCardViewModel in viewModel.SkillCardsInventory)
            {
                CreateSkillCard(skillCardViewModel);
            }

            UpdateHeightContainerCard();
            viewModel.SkillCardsInventory.ObserveAdd().Subscribe(e =>
            {
                CreateSkillCard(e.Value);
                UpdateHeightContainerCard();
            }).AddTo(ref d);
            viewModel.SkillCardsInventory.ObserveRemove().Subscribe(e =>
            {
                DestroySkillCard(e.Value);
                UpdateHeightContainerCard();
            }).AddTo(ref d);


            foreach (var skillPlanViewModel in viewModel.SkillPlansInventory)
            {
                CreateSkillPlan(skillPlanViewModel);
            }

            UpdateHeightContainerPlan();
            viewModel.SkillPlansInventory.ObserveAdd()
                .Subscribe(e =>
                {
                    CreateSkillPlan(e.Value);
                    UpdateHeightContainerPlan();
                })
                .AddTo(ref d);
            viewModel.SkillPlansInventory.ObserveRemove()
                .Subscribe(e =>
                {
                    DestroySkillPlan(e.Value);
                    UpdateHeightContainerPlan();
                })
                .AddTo(ref d);

            _disposable = d.Build();
        }

        private void CreateSkillCard(SkillCardViewModel viewModel)
        {
            var prefabSkillCardPath =
                $"Prefabs/UI/MainMenu/ScreenInventory/SkillCard";
            var skillPrefab = Resources.Load<SkillCardBinder>(prefabSkillCardPath);
            var createdSkill = Instantiate(skillPrefab, containerCards);
            createdSkill.Bind(viewModel);
            createdSkill.transform.SetSiblingIndex(0);
            _createdSkillCardMap[viewModel.IdSkillCard] = createdSkill;
        }

        private void CreateSkillPlan(SkillPlanViewModel viewModel)
        {
            var prefabSkillPlanPath =
                $"Prefabs/UI/MainMenu/ScreenInventory/SkillPlan";
            var skillPrefab = Resources.Load<SkillPlanBinder>(prefabSkillPlanPath);
            var createdSkill = Instantiate(skillPrefab, containerPlans);
            createdSkill.Bind(viewModel);
            _createdSkillPlanMap[viewModel.IdSkillPlan] = createdSkill;
        }

        private void DestroySkillCard(SkillCardViewModel viewModel)
        {
            if (_createdSkillCardMap.TryGetValue(viewModel.IdSkillCard, out var skillCardBinder))
            {
                Destroy(skillCardBinder.gameObject);
                _createdSkillCardMap.Remove(viewModel.IdSkillCard);
            }
        }

        private void DestroySkillPlan(SkillPlanViewModel viewModel)
        {
            if (_createdSkillPlanMap.TryGetValue(viewModel.IdSkillPlan, out var skillPlanBinder))
            {
                Destroy(skillPlanBinder.gameObject);
                _createdSkillPlanMap.Remove(viewModel.IdSkillPlan);
            }
        }


        private void UpdateHeightContainerCard()
        {

            UpdateContainer(
                containerCards.GetComponent<RectTransform>(),
                _viewModel.SkillCardsInventory.Count,
                CardsContainerConsts
            );
        }

        private void UpdateHeightContainerPlan()
        {
            UpdateContainer(
                containerPlans.GetComponent<RectTransform>(),
                _viewModel.SkillPlansInventory.Count,
                PlansContainerConsts
            );
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateHeightContainerCard();
            UpdateHeightContainerPlan();
            btnBlacksmith.onClick.AddListener(OnOpenPopupBlacksmith);
        }

        private void OnDisable()
        {
            btnBlacksmith.onClick.RemoveListener(OnOpenPopupBlacksmith);
        }

        private void OnOpenPopupBlacksmith()
        {
            _viewModel.RequestPopupBlacksmith();
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}