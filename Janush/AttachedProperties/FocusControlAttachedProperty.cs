// Copyright(c) Łukasz Szwedt. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Janush
{
    /// <summary>
    /// Brings the HTML label functionality to the <see cref="Label"/> control.
    /// </summary>
    public class FocusControlAttachedProperty : BaseAttachedProperty<FocusControlAttachedProperty, UIElement>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Don't do this in design time
            if (DesignerProperties.GetIsInDesignMode(sender))
            {
                return;
            }

            // If we have a label and control...
            if (sender is Label label && e.NewValue is UIElement control)
            {
                // Set label's target
                label.Target = control;

                // Listen for label clicks
                label.PreviewMouseLeftButtonDown -= Label_PreviewMouseLeftButtonDown;
                label.PreviewMouseLeftButtonDown += Label_PreviewMouseLeftButtonDown;
            }
        }

        private void Label_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // If we have a label that has target set
            if (sender is Label label && label.Target != null)
            {
                // If target is a toggle button...
                if (label.Target is ToggleButton toggleButton)
                {
                    // Toggle check state
                    toggleButton.IsChecked ^= true;
                }
                // If target is a text box...
                else if (label.Target is TextBox textBox)
                {
                    // Move caret at the end
                    textBox.CaretIndex = textBox.Text.Length;
                }
                // If target is a password box...
                else if (label.Target is PasswordBox passwordBox)
                {
                    // Select entire text
                    passwordBox.SelectAll();
                }

                // Focus target element
                label.Target.Focus();
            }
        }
    }
}
