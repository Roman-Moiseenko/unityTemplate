using System;
using System.Collections.Generic;
using Game.Common;
using Game.GameRoot.ImageManager;
using UnityEngine;

namespace Game.GamePlay.View.Grounds
{
    public class BoardBinder : MonoBehaviour
    {
        [SerializeField] private GameObject place;
        [SerializeField] private Transform container;
        private BoardViewModel _viewModel;

        private const float DeltaSide = 0.38f;
        private const float DeltaInAngle = 0.34f;
        private const float DeltaOutAngle = 0.35f;

        private Dictionary<string, BoardWallBinder> _createWallsMap = new();
        public void Bind(BoardViewModel viewModel)
        {

            _viewModel = viewModel;
            var imageManager = GameObject.Find(AppConstants.IMAGE_MANAGER).GetComponent<ImageManagerBinder>();
            var position = Math.Abs((viewModel.Position.CurrentValue.x + viewModel.Position.CurrentValue.y) % 2);

            var meshRenderer = place.GetComponent<MeshRenderer>();
            var matBlock = new MaterialPropertyBlock();
            meshRenderer.GetPropertyBlock(matBlock);
            
            matBlock.SetTexture("_Plane", imageManager.GetGround(viewModel.ConfigId)); //viewModel.ConfigId
            matBlock.SetInt("_Odd", position);
            matBlock.SetInt("_IsBoard", 1);
            
            matBlock.SetInt("_LeftSide", viewModel.BoardEntity.Origin.LeftSide ? 1 : 0);
            matBlock.SetInt("_TopSide", viewModel.BoardEntity.Origin.TopSide ? 1 : 0);
            matBlock.SetInt("_RightSide", viewModel.BoardEntity.Origin.RightSide ? 1 : 0);
            matBlock.SetInt("_BottomSide", viewModel.BoardEntity.Origin.BottomSide ? 1 : 0);
            
            matBlock.SetInt("_LeftOutAngle", viewModel.BoardEntity.Origin.LeftOutAngle ? 1 : 0);
            matBlock.SetInt("_TopOutAngle", viewModel.BoardEntity.Origin.TopOutAngle ? 1 : 0);
            matBlock.SetInt("_RightOutAngle", viewModel.BoardEntity.Origin.RightOutAngle ? 1 : 0);
            matBlock.SetInt("_BottomOutAngle", viewModel.BoardEntity.Origin.BottomOutAngle ? 1 : 0);
            
            matBlock.SetInt("_LeftInAngle", viewModel.BoardEntity.Origin.LeftInAngle ? 1 : 0);
            matBlock.SetInt("_TopInAngle", viewModel.BoardEntity.Origin.TopInAngle ? 1 : 0);
            matBlock.SetInt("_RightInAngle", viewModel.BoardEntity.Origin.RightInAngle ? 1 : 0);
            matBlock.SetInt("_BottomInAngle", viewModel.BoardEntity.Origin.BottomInAngle ? 1 : 0);
            
            meshRenderer.SetPropertyBlock(matBlock);
            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x,
                -1,
                viewModel.Position.CurrentValue.y
            );
            
            //TODO  Создаем Из превабов стороны и углы по списку Сторон из ViewModel

            foreach (var wallViewModel in viewModel.Walls)
            {
                CreateBoard(wallViewModel);
            }
            
        }
        
        
        private void CreateBoard(BoardWallViewModel viewModel)
        {
            var prefabWallPath = $"Prefabs/Gameplay/Grounds/Boards/{viewModel.ConfigId}"; //Перенести в настройки уровня {groundType}
            var wallPrefab = Resources.Load<BoardWallBinder>(prefabWallPath);
            var createdWall = Instantiate(wallPrefab, container.transform);
            createdWall.Bind(viewModel);
            _createWallsMap[viewModel.Id] = createdWall;
        }

        private void OnDestroy()
        {
            foreach (var wallViewModel in _viewModel.Walls)
            {
                if (_createWallsMap.TryGetValue(wallViewModel.Id, out var wallBinder))
                {
                    Destroy(wallBinder.gameObject);
                    _createWallsMap.Remove(wallViewModel.Id);
                }
            }
        }
    }
}