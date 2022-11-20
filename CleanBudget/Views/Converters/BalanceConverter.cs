using System;
using System.Globalization;
using System.Windows.Data;

namespace CleanBudget.Views.Converters
{
    public class BalanceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 3) return "+" + values[1] + " " + values[2];
            else return values[0] + " " + values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
