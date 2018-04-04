using System;
using System.Globalization;
using Xamarin.Forms;

namespace Xamrealm.Converters
{
    public class CompleteRestoreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var completed = (bool)value;
            return completed ? "Restore" : "Complete";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
