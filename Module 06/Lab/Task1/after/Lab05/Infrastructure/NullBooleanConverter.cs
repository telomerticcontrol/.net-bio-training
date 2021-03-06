﻿using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Markup;

namespace Lab05.Infrastructure
{
    /// <summary>
    /// This converts an object value to a boolean
    /// Taken from MVVMHelpers (http://mvvmhelpers.codeplex.com)
    /// </summary>
    [ValueConversion(typeof(object), typeof(bool))]
    public class NullBooleanConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Mapping value for null - defaults to false.
        /// </summary>
        public bool NullTreatment { get; set; }
        /// <summary>
        /// Mapping value for non-null, defaults to true.
        /// </summary>
        public bool NonNullTreatment { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public NullBooleanConverter()
        {
            NullTreatment = false;
            NonNullTreatment = true;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new ArgumentException("Bad type conversion for NullBooleanConverter");
            return (value == null) ? NullTreatment : NonNullTreatment;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the converter
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new NullBooleanConverter {NullTreatment = this.NullTreatment, NonNullTreatment = this.NonNullTreatment};
        }
    }
}