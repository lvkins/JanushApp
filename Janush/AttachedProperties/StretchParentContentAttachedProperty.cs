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
