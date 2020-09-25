using System;
using System.Globalization;
using System.Windows.Data;

namespace BeatSaberSongManager.Converters
{
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is TimeSpan timeSpan)
            {
                if (timeSpan.Hours > 0)
                    return timeSpan.ToString(@"hh\:mm\:ss", CultureInfo.CurrentCulture);
                else
                    return timeSpan.ToString(@"mm\:ss", CultureInfo.CurrentCulture);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
