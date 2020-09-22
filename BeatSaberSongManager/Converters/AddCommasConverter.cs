using System;
using System.Globalization;
using System.Windows.Data;

namespace BeatSaberSongManager.Converters
{
    public class AddCommasConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return null;

            return string.Format(CultureInfo.CurrentCulture, "{0:n0}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
