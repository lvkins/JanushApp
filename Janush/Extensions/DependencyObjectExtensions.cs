using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Janush
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

        public static IEnumerable<DependencyObject> Childrens(this DependencyObject parent)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);

            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is DependencyObject dependencyObject)
                {
                    yield return dependencyObject;
                }

                foreach (var other in child.Childrens())
                {
                    yield return other;
                }
            }
        }
    }
}
