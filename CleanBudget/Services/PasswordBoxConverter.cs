﻿using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls;

namespace CleanBudget.Services
{
    public class PasswordBoxConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return new Tuple<object, object>(values[0], values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
