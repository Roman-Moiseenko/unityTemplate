﻿namespace MVVM.UI
{
    public interface IWindowBinder
    {
        void Bind(WindowViewModel viewModel);

        void Close();

        void Show();

        void Hide();
    }
}