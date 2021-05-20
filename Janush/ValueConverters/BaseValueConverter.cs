// Copyright(c) Łukasz Szwedt. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Janush
{
    /// <summary>
    /// A base value converter for use in XAML files.
    /// </summary>
    /// <typeparam name="T">The type of this value converter.</typeparam>
    public abstract class BaseValueConverter<T> : MarkupExtension, IValueConverter
        where T : class, new()
    {
        #region Private Members

        /// <summary>
        /// A single instance of the value converter.
        /// </summary>
        private static T Converter = null;

        #endregion

        #region Markup Extension Methods

        /// <summary>
        /// Provides a single instance of the value converter.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider) => Converter ?? (Converter = new T());

        #endregion

        #region Interface Implementation

        /// <summary>
        /// The method to convert one type to another.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        /// <summary>
        /// The method to convert a type back to it's original type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

        #endregion
    }
}
