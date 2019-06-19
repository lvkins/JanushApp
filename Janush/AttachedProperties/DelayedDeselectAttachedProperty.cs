using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Janush
{
    /// <summary>
    /// An attached property that deselects the selected <see cref="ListBoxItem"/> after certain amount of time.
    /// </summary>
    public class DelayedDeselectAttachedProperty : BaseAttachedProperty<DelayedDeselectAttachedProperty, bool>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Don't do this in design time
            if (DesignerProperties.GetIsInDesignMode(sender))
            {
                return;
            }

            // If we have a control...
            if (sender is ListBox listBox)
            {
                // Hook into the event
                listBox.RemoveHandler(ListBoxItem.SelectedEvent, (RoutedEventHandler)OnListBoxItemSelectedAsync);
                listBox.AddHandler(ListBoxItem.SelectedEvent, (RoutedEventHandler)OnListBoxItemSelectedAsync, true);
            }
        }

        /// <summary>
        /// Called when an <see cref="ListBoxItem.SelectedEvent"/> is raised.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnListBoxItemSelectedAsync(object sender, RoutedEventArgs e)
        {
            // If we have an item...
            if (e.OriginalSource is ListBoxItem item)
            {
                // Wait for 5 seconds
                await Task.Delay(TimeSpan.FromSeconds(5));

                // Deselect
                item.IsSelected = false;
            }
        }
    }
}
