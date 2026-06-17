using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.SkillStates;
using Game.GamePlay.View.Frames.SkillFrames;
using Game.GameRoot.Queries.SkillQueries;
using Game.Settings.Gameplay.Entities.Skill;
using Game.State.Gameplay;
using Game.State.Maps.Roads;
using MVVM.CMD;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class FrameSkillService : IDisposable
    {
        public IObservableCollection<FrameSkillViewModel> ViewModels => _viewModels;   
        private readonly ObservableList<FrameSkillViewModel> _viewModels = new();
        private readonly DIContainer _container;
        private readonly GameplayStateProxy _gameplayState;
        private readonly PlacementService _placementService;
        private readonly SkillsService _skillService;
        private readonly RoadsService _roadsService;
        private FrameSkillViewModel _viewModel;

        private readonly FsmSkill _fsmSkill;
        private DisposableBag _disposables = new();
        private readonly IQueryProcessor _qrc;

        //Вычисляемые параметры для моделей в Binder
        public readonly ReactiveProperty<bool> IsPlacement = new(false);
        public readonly ReactiveProperty<Vector2Int> Direction = new(Vector2Int.zero);
        public List<RoadPoint> Cells = new(); //TODO доделать, (position, direction) Observable
        
        public FrameSkillService(
            GameplayStateProxy gameplayState,
            PlacementService placementService,
            SkillsService skillService,
            RoadsService roadsService,
            SkillsSettings skillsSettings, //Удалить?
            IQueryProcessor qrc,
            DIContainer container
        )
        {
            _container = container;
            _gameplayState = gameplayState;
            _placementService = placementService;
            _skillService = skillService;
            _roadsService = roadsService;
            _qrc = qrc;
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
                    _viewModel.IsEnable.Value = true;
                }

                if (newState.GetType() == typeof(FsmSkillNone)) ClearViewModel();
                
            }).AddTo(ref _disposables);
            
            _fsmSkill.PositionCursor.Subscribe(newPosition => 
                {
                    if (_fsmSkill.IsTarget())
                    {
                        if (_viewModel.OnRoad)
                        {
                            _fsmSkill.Position.Value = new Vector2Int(
                                Mathf.FloorToInt(newPosition.x + 0.5f),
                                Mathf.FloorToInt(newPosition.y + 0.5f)
                            );
                        }
                        else
                        {
                            _fsmSkill.Position.Value = newPosition;
                        }
                        MoveFrame(newPosition);
                    }
                }
            ).AddTo(ref _disposables);
            
        }

        private void MoveFrame(Vector2 position)
        {         
            
            if (_viewModel.OnRoad)
            {
                var vectorInt = new Vector2Int(
                    Mathf.FloorToInt(position.x + 0.5f),
                    Mathf.FloorToInt(position.y + 0.5f)
                );
                _viewModel.MoveFrame(vectorInt);
            }
            else
            {
                _viewModel.MoveFrame(position);
            }
            
            //_fsmSkill.Position.Value = position;
            CheckPlacement(position);
        }

        /**
         * Проверяем размещение для _viewModel
         */
        private void CheckPlacement(Vector2 position)
        {
            if (!_viewModel.OnRoad)
            {
                IsPlacement.Value = true;
                return;
            }
            var vectorInt = new Vector2Int(
                Mathf.FloorToInt(position.x + 0.5f),
                Mathf.FloorToInt(position.y + 0.5f)
            );
            IsPlacement.Value = _placementService.IsRoad(vectorInt);
            if (!_placementService.IsRoad(vectorInt)) return;
            
            if (_viewModel.MultiCells == 0)
            {
                Direction.Value = _placementService.GetRoadDirectionNext(vectorInt);
                _fsmSkill.Direction.Value = Direction.Value;
            }
            else
            {
                //TODO Вычисляем координаты всех ячеек и их направление
                Cells = new List<RoadPoint>();
                _fsmSkill.Cells = Cells.ToList();
            }
        }


        private void CreateViewModel(string configId)
        {
            if (_viewModels.Count > 0) ClearViewModel();

            var query = new QueryInfoSkill(configId);
            var settings = _qrc.Request<QueryInfoSkill, SkillSettings>(query);
            var param = _skillService.GetParameters(configId);
            _viewModel = new FrameSkillViewModel(configId, settings, this, param);
            _viewModels.Add(_viewModel);
        }

        public void Dispose()
        {
            ClearViewModel();
            IsPlacement?.Dispose();
            _disposables.Dispose();
        }

        private void ClearViewModel()
        {
            _viewModel?.Dispose();
            _viewModel = null;
            _viewModels.Clear();
        }
    }
}
