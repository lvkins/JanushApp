// Copyright(c) Łukasz Szwedt. All rights reserved.
// Licensed under the MIT license.

using System.Windows.Input;

namespace Janush
{
    /// <summary>
    /// A interface representing a window view model.
    /// </summary>
    public interface IWindowViewModel
    {
        #region Commands

        /// <summary>
        /// The command to open the window.
        /// </summary>
        ICommand OpenCommand { get; set; }

        /// <summary>
        /// The command to close the window.
        /// </summary>
        ICommand CloseCommand { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Opens a window.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes a window.
        /// </summary>
        void Close();

        #endregion
    }
}
