using System.ComponentModel;

namespace Janush
{
    /// <summary>
    /// A base for the view models across the application that have a need to implement property changed behavior.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region Public Events

        /// <summary>
        /// Event raised when any child property value has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        #endregion

        #region Public Methods

        /// <summary>
        /// A shortcut method to raise the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="name">The name of the changed property.</param>
        public void OnPropertyChanged(string name)
        {
            // Raise property changed event
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
