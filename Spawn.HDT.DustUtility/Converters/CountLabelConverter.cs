using System;
using System.Globalization;
using System.Windows.Data;

namespace Spawn.HDT.DustUtility.Converters
{
    public class CountLabelConverter : IValueConverter
    {
        #region Convert
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{value}x";
        }
        #endregion

        #region ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}