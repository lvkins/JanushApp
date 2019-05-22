using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PromoSeeker
{
    /// <summary>
    /// An attached property that allows <see cref="ScrollBar"/> canvas offset to be set directly through the <see cref="ScrollViewer"/>.
    /// </summary>
    public class ScrollBarOffsetAttachedProperty : BaseAttachedProperty<ScrollBarOffsetAttachedProperty, double>
    {
        /// <summary>
        /// The scrollbar offset.
        /// </summary>
        private double _offset;

        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // If we have a control...
            if (sender is ScrollViewer scrollViewer)
            {
                // Store the offset value
                _offset = (double)e.NewValue;

                // Hook into event
                scrollViewer.Loaded -= ScrollViewer_Loaded;
                scrollViewer.Loaded += ScrollViewer_Loaded;
            }
        }

        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            // If have no scrollviewer...
            if (!(sender is ScrollViewer scrollViewer))
            {
                return;
            }

            // If horizontal scrollbar is visible...
            if (scrollViewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible)
            {
                // If the horizontal scrollbar was found within scrollviewer template...
                if (scrollViewer.Template.FindName("PART_HorizontalScrollBar", scrollViewer) is ScrollBar scrollBar)
                {
                    // Apply horizontal offset
                    scrollBar.Width = scrollViewer.ActualWidth - _offset;
                }
            }

            // If vertical scrollbar is visible...
            if (scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible)
            {
                // If the vertical scrollbar was found within scrollviewer template...
                if (scrollViewer.Template.FindName("PART_VerticalScrollBar", scrollViewer) is ScrollBar scrollBar)
                {
                    // Apply vertical offset
                    scrollBar.Height = scrollViewer.ActualHeight - _offset;
                }
            }
        }

        private void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }
    }
}
