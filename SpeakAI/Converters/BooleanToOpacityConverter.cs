using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace SpeakAI.Converters
{
    public class BooleanToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible)
            {
                return isVisible ? 1.0 : 0.0; // 1.0 for visible, 0.0 for hidden
            }
            return 0.0; // Default to hidden if value is not a boolean
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}