﻿using UnityEngine;

namespace Game.GamePlay.View.Castle
{
    public class CastleBinder : MonoBehaviour
    {
        public void Bind(CastleViewModel viewModel)
        {
            transform.position = new Vector3(
                viewModel.Position.x,
                0,
                viewModel.Position.y
            );
        }
    }
}