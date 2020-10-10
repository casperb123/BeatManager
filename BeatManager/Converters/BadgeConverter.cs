using System;
using System.Globalization;
using System.Windows.Data;

namespace BeatManager.Converters
{
    public class BadgeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                if (intValue > 999)
                    return "999+";
                else if (intValue <= 0)
                    return null;
                else
                    return intValue;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
