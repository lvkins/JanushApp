using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Janush
{
    /// <summary>
    /// An attached property that causes to be scrolled into view on owning <see cref="ListBox"/>.
    /// </summary>
    public class ScrollToSelectedAttachedProperty : BaseAttachedProperty<ScrollToSelectedAttachedProperty, bool>
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
                // Hook into the selection changed
                listBox.SelectionChanged -= ListBox_SelectionChanged;
                listBox.SelectionChanged += ListBox_SelectionChanged;
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && e.AddedItems.Count != 0)
            {
                // Get selected item
                var item = listBox.ItemContainerGenerator.ContainerFromIndex(listBox.SelectedIndex);

                // Get scroll viewer
                var scrollViewer = item?.Parents().OfType<ScrollViewer>().FirstOrDefault();

                // Scroll to the end, so that when we ScrollIntoView, the selected item appears on the top
                scrollViewer?.ScrollToBottom();

                // Bring item to view
                listBox.ScrollIntoView(e.AddedItems[0]);
            }
        }
    }
}
