using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace SpeakAI.Converters
{
    public class PremiumBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "#FFD700" : "#D6DBDF"; // Gold Border for VIP
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
