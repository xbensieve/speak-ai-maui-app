using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace SpeakAI.Converters
{
    public class RadioButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // Check if the value and parameter are not null, then perform the comparison
                return value?.ToString() == parameter?.ToString();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as necessary
                System.Diagnostics.Debug.WriteLine($"Error in Convert: {ex.Message}");
                return false; // Default to false to avoid crashes
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // Ensure value is a boolean before performing the operation
                return (value is bool boolValue && boolValue) ? parameter?.ToString() : null;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as necessary
                System.Diagnostics.Debug.WriteLine($"Error in ConvertBack: {ex.Message}");
                return null; // Default to null to avoid crashes
            }
        }
    }
}
