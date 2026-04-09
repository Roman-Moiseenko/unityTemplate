using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.SkillStates;
using Game.GamePlay.View.Frames;
using Game.Settings.Gameplay.Entities.Skill;
using Game.State.Gameplay;
using MVVM.CMD;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class FrameSkillService
    {
        public IObservableCollection<FrameSkillViewModel> ViewModels => _viewModels;   
        private readonly ObservableList<FrameSkillViewModel> _viewModels = new();
        private readonly DIContainer _container;
        private readonly GameplayStateProxy _gameplayState;
        private readonly PlacementService _placementService;
        private readonly SkillsService _skillService;
        private readonly RoadsService _roadsService;
        private FrameSkillViewModel _viewModel;
        public bool IsPlacement;
        private readonly FsmSkill _fsmSkill;

        public FrameSkillService(
            GameplayStateProxy gameplayState,
            PlacementService placementService,
            SkillsService skillService,
            RoadsService roadsService,
            SkillsSettings skillsSettings,
            IQueryProcessor qrc,
            DIContainer container
        )
        {
            _container = container;
            _gameplayState = gameplayState;
            _placementService = placementService;
            _skillService = skillService;
            _roadsService = roadsService;
            
            _fsmSkill = container.Resolve<FsmSkill>();
            
            //Подписка на те варианты, которые влияют на FrameSkill 
            _fsmSkill.Fsm.StateCurrent.Subscribe(newState =>
            {
                //Debug.Log(newState.GetType());
                if (newState.GetType() == typeof(FsmSkillBegin))
                {
                    CreateViewModel(_fsmSkill.GetConfigId());
                }

                if (newState.GetType() == typeof(FsmSkillSetTarget) && _viewModel != null)
                {
                    _viewModel.Enable.Value = true;
                }

                if (newState.GetType() == typeof(FsmSkillNone)) _viewModels.Clear();
                
            });
            _fsmSkill.Position.Subscribe(newPosition => 
                {
                    if (_fsmSkill.IsTarget()) MoveFrame(newPosition); //Переносим объект
                }
            );
            
        }

        public void MoveFrame(Vector2Int position)
        {
            _viewModel.MoveFrame(position);
            CheckPlacement();
        }

        /**
         * Проверяем размещение для _viewModel
         */
        private void CheckPlacement()
        {
            IsPlacement = true;
        }

        public void CreateViewModel(string configId)
        {
            if (_viewModels.Count > 0) _viewModels.Clear();
            
            _viewModel = new FrameSkillViewModel(configId);
            _viewModels.Add(_viewModel);
        }
    }
}