// Copyright(c) Łukasz Szwedt. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Janush
{
    /// <summary>
    /// An attached property for the <see cref="TextBox"/> controls that changes it's behavior
    /// so that a valid time value can be typed in for a <see cref="TimeSpan"/> dependency property.
    /// </summary>
    public class TimeTextAttachedProperty : BaseAttachedProperty<TimeTextAttachedProperty, TimeSpan>
    {
        /// <summary>
        /// A format of how time is represented.
        /// </summary>
        private static readonly string _format = @"hh\:mm\:ss";

        #region Additional Attached Property

        public static int GetMinValue(DependencyObject obj)
        {
            return (int)obj.GetValue(MinValueProperty);
        }

        public static void SetMinValue(DependencyObject obj, int value)
        {
            obj.SetValue(MinValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.RegisterAttached("MinValue", typeof(int), typeof(TimeTextAttachedProperty), new PropertyMetadata(default(int)));

        #endregion

        /// <summary>
        /// Sets up the textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Don't do this in design time
            if (DesignerProperties.GetIsInDesignMode(sender))
            {
                return;
            }


            // If we have a textbox...
            if (sender is TextBox textBox)
            {
                // Subscribe to events
                textBox.GotFocus -= TextBox_GotFocus;
                textBox.GotFocus += TextBox_GotFocus;
                textBox.LostFocus -= TextBox_LostFocus;
                textBox.LostFocus += TextBox_LostFocus;
                textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
                textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            }
        }

        /// <summary>
        /// Fills the current textbox text property accordingly to the current <see cref="TimeSpan"/> dependency property value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // If we have a textbox...
            if (sender is TextBox textBox)
            {
                // Update textbox value to the current property value
                textBox.Text = GetValue(textBox).ToString(_format, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Validates the input on lost focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // If we have no textbox...
            if (!(sender is TextBox textBox))
            {
                return;
            }

            // If the input is valid...
            if (TimeSpan.TryParseExact(textBox.Text, _format, CultureInfo.InvariantCulture, out var result) &&
                result > TimeSpan.Zero && result.TotalSeconds >= GetMinValue(textBox))
            {
                // Update value
                SetValue(textBox, result);
            }

            // Clear textbox content
            textBox.Text = string.Empty;
        }

        /// <summary>
        /// Lose textbox focus on return.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // If we have no textbox...
            if (!(sender is TextBox textBox))
            {
                return;
            }

            // If key is a return...
            if (e.Key == Key.Return)
            {
                // Kill logical focus
                FocusManager.SetFocusedElement(FocusManager.GetFocusScope(textBox), null);

                // Kill keyboard focus
                Keyboard.ClearFocus();
            }
        }
    }
}
