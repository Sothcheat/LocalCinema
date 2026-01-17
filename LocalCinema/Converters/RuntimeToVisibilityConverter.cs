using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace LocalCinema.Converters
{
    public class RuntimeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int runtime && runtime > 0)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}