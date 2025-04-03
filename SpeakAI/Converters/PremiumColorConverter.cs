using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace SpeakAI.Converters
{
    public class PremiumColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "#FFE4B5" : "White"; // Light Gold for VIP Courses
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
