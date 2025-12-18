using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.Commands.GroundCommands;
using Game.GamePlay.View.Grounds;
using Game.Settings.Gameplay.Grounds;
using Game.State.Entities;
using Game.State.Maps.Grounds;
using Game.State.Root;
using MVVM.CMD;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Services
{
    public class GroundsService
    {
        private readonly string _configIdDefault;
        private readonly ICommandProcessor _cmd;
        private readonly GameplayStateProxy _gameplayState;

        private readonly ObservableList<GroundViewModel> _allGrounds = new();
        private readonly Dictionary<int, GroundViewModel> _groundsMap = new();

        private Dictionary<int, int> _mapGrounds = new();

        private readonly ObservableList<BoardViewModel> _allBoards = new();
        private readonly Dictionary<int, BoardViewModel> _boardsMap = new();

        private List<BoardEntityData> _listBoardEntityData = new();
        
        //     private readonly Dictionary<string, GroundSettings> _groundSettingsMap = new();

        public IObservableCollection<GroundViewModel> AllGrounds =>
            _allGrounds; //Интерфейс менять нельзя, возвращаем через динамический массив

        public IObservableCollection<BoardViewModel> AllBoards => _allBoards;

        public GroundsService(
            IObservableCollection<GroundEntity> grounds,
            string configIdDefault,
            // GroundSettings groundSettings,
            ICommandProcessor cmd,
            GameplayStateProxy gameplayState
        )
        {
            _configIdDefault = configIdDefault;
            _cmd = cmd;
            _gameplayState = gameplayState;


            foreach (var ground in grounds)
            {
                CreateGroundViewModel(ground);
            }

            GenerateBoardsList();
            CreateBoardViewModels();
            //Debug.Log(JsonConvert.SerializeObject(_listBoardEntityData, Formatting.Indented));
            //TODO Обсчет Границ

            //Подписка на добавление новых view-моделей текущего класса
            grounds.ObserveAdd().Subscribe(e =>
            {
                CreateGroundViewModel(e.Value);
                GenerateBoardsList();
                CreateBoardViewModels();
                //Debug.Log(JsonConvert.SerializeObject(_listBoardEntityData, Formatting.Indented));
                //TODO Переобсчет Границ
            });
            // и на удаление
            grounds.ObserveRemove().Subscribe(e =>
            {
                RemoveGroundViewModel(e.Value);
                GenerateBoardsList();
                CreateBoardViewModels();
                //TODO Переобсчет Границ
            });
        }

        public bool PlaceGround(Vector2Int position)
        {
            // Debug.Log("Строим землю");
            var command = new CommandCreateGround(_configIdDefault, position);
            return _cmd.Process(command);
        }

        public bool DeleteGround(Vector2Int position)
        {
            var command = new CommandRemoveGround(position);
            return _cmd.Process(command);
        }

        /**
         * 1. По параметрам создается сущность GroundEntity
         * 2. Оборачивается Proxy для навешивания реактивности и событий
         * 3. На основе Proxy сущности создается view-модель
         * 4. Модель добавляем в словарь всех моделей данного класса
         * 5. Кешируем Id и view-модели
         */
        private void CreateGroundViewModel(GroundEntity groundEntity)
        {
            var groundViewModel = new GroundViewModel(groundEntity, this); //3
            _allGrounds.Add(groundViewModel); //4
            _groundsMap[groundEntity.UniqueId] = groundViewModel;
            //_mapGrounds.Add(groundEntity.Position.Value.x, groundEntity.Position.Value.y);
        }

        private void CreateBoardViewModels()
        {
            //Debug.Log("CreateBoardViewModels = " + _allBoards.Count);
            foreach (var boardViewModel in _allBoards.ToList())
            {
                var boardEntityData = _listBoardEntityData.Find(e => e.Position == boardViewModel.Position.CurrentValue);
                if (boardEntityData == null)
                {
                    _allBoards.Remove(boardViewModel);
                }
                else
                {
                    //Проверяем данные
                    if (!boardViewModel.BoardEntity.EqualsData(boardEntityData))
                    {
                        _allBoards.Remove(boardViewModel);
                        var model = new BoardViewModel(new BoardEntity(boardEntityData));
                        _allBoards.Add(model);
                    }

                    _listBoardEntityData.Remove(boardEntityData);
                }

            }
            //Debug.Log("_listBoardEntityData = " + _listBoardEntityData.Count);
            foreach (var boardEntityData in _listBoardEntityData)
            {
                var model = new BoardViewModel(new BoardEntity(boardEntityData));
                _allBoards.Add(model);
            }
        }
        
        

        /**
         * Удаляем объект из списка моделей и из кеша
         */
        private void RemoveGroundViewModel(GroundEntity groundEntity)
        {
            if (_groundsMap.TryGetValue(groundEntity.UniqueId, out var groundViewModel))
            {
                _allGrounds.Remove(groundViewModel);
                _groundsMap.Remove(groundEntity.UniqueId);
            }
        }

        private void GenerateBoardsList()
        {
            _listBoardEntityData.Clear();
            
            foreach (var groundViewModel in _allGrounds)
            {
                var p = groundViewModel.Position.CurrentValue;

                
                
                if (!IsGround(new Vector2Int(p.x - 1, p.y - 1))) //Верхний правый угол
                {
                    var board = CreateBoard(new Vector2Int(p.x - 1, p.y - 1));
                    board.BottomAngle = true;
                }
                if (!IsGround(new Vector2Int(p.x - 1, p.y))) //Правая сторона
                {
                    var board = CreateBoard(new Vector2Int(p.x - 1, p.y));
                    board.LeftSide = true;
                }
                if (!IsGround(new Vector2Int(p.x - 1, p.y + 1))) //Нижний правый угол
                {
                    var board = CreateBoard(new Vector2Int(p.x - 1, p.y + 1));
                    board.LeftAngle = true;
                }
                if (!IsGround(new Vector2Int(p.x, p.y + 1))) //Нижняя сторона
                {
                    var board = CreateBoard(new Vector2Int(p.x, p.y + 1));
                    board.TopSide = true;
                }
                if (!IsGround(new Vector2Int(p.x + 1, p.y + 1))) //Нижний левый угол
                {
                    var board = CreateBoard(new Vector2Int(p.x + 1, p.y + 1));
                    board.TopAngle = true;
                }
                if (!IsGround(new Vector2Int(p.x + 1, p.y))) //Левая сторона
                {
                    var board = CreateBoard(new Vector2Int(p.x + 1, p.y));
                    board.RightSide = true;
                }
                if (!IsGround(new Vector2Int(p.x + 1, p.y - 1))) //Верхний левый угол
                {
                    var board = CreateBoard(new Vector2Int(p.x + 1, p.y - 1));
                    board.RightAngle = true;
                }
                if (!IsGround(new Vector2Int(p.x, p.y - 1))) //Верхняя сторона
                {
                    var board = CreateBoard(new Vector2Int(p.x, p.y - 1));
                    board.BottomSide = true;
                }
            }
        }

        private bool IsGround(Vector2Int position)
        {
            return _allGrounds.Any(ground => ground.Position.CurrentValue == position);
        }

        private BoardEntityData CreateBoard(Vector2Int position)
        {
            if (position == new Vector2Int(-1, 2))
            {
                Debug.Log(" * ** * *");
            }
            //Проверяем есть ли по этим координатам боковина
            foreach (var boardData in _listBoardEntityData)
            {
                if (boardData.Position == position) return boardData;
            }

            var boardEntityData = new BoardEntityData
            {
                Position = position,
                UniqueId = _gameplayState.CreateEntityID(),
                ConfigId = _configIdDefault,
            };


            _listBoardEntityData.Add(boardEntityData);
            return boardEntityData;
        }
    }
}