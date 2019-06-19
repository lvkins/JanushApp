using System;
using System.Windows.Input;

namespace Janush
{
    /// <summary>
    /// A command that runs a single action.
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Private Members

        /// <summary>
        /// The action to run.
        /// </summary>
        private readonly Action _action;

        #endregion

        #region Public Events

        /// <summary>
        /// An event that fires when the <see cref="CanExecute(object)"/>
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        #endregion

        #region Constructor

        /// <summary>
        /// The action constructor.
        /// </summary>
        /// <param name="action">The action to be executed.</param>
        public RelayCommand(Action action)
        {
            _action = action;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>A relay command always returns <see langword="true"/>.</returns>
        public bool CanExecute(object parameter) => true;

        /// <summary>
        /// Runs an action when the command was invoked.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter) => _action();

        #endregion
    }

    /// <summary>
    /// A command that runs a single action that takes in a parameter.
    /// </summary>
    public class RelayParamCommand : ICommand
    {
        #region Private Members

        /// <summary>
        /// The action to run.
        /// </summary>
        private readonly Action<object> _action;

        #endregion

        #region Public Events

        /// <summary>
        /// An event that fires when the <see cref="CanExecute(object)"/>
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        #endregion

        #region Constructor

        /// <summary>
        /// The action constructor.
        /// </summary>
        /// <param name="action">The action to be executed.</param>
        public RelayParamCommand(Action<object> action)
        {
            _action = action;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>A relay command always returns <see langword="true"/>.</returns>
        public bool CanExecute(object parameter) => true;

        /// <summary>
        /// Runs an action when the command was invoked.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter) => _action(parameter);

        #endregion
    }
}