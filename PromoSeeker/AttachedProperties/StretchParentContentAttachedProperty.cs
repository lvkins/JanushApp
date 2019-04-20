using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

    public class StretchParentContentProperty : BaseAttachedProperty<StretchParentContentProperty, string>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var presenter = sender.Parents().OfType<ContentPresenter>().FirstOrDefault();
            if (presenter != null)
            {
                presenter.HorizontalAlignment = (HorizontalAlignment)e.NewValue;
            }
        }

        public override void OnValueUpdated(DependencyObject sender, object value)
        {
            base.OnValueUpdated(sender, value);
        }
    }
}
