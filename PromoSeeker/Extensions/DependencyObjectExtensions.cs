using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace PromoSeeker
{
    public static class DependencyObjectExtensions
    {
        public static IEnumerable<DependencyObject> Parents(this DependencyObject child)
        {
            var parent = VisualTreeHelper.GetParent(child);
            while (parent != null)
            {
                yield return parent;
                child = parent;
                parent = VisualTreeHelper.GetParent(child);
            }
        }
    }
}
