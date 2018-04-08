using System;
using System.Globalization;
using Xamarin.Forms;
using Xamrealm.Base;

namespace Xamrealm.Converters
{
    public class NullEmptyStringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string; 
            return !str.IsNullOrWhiteSpace();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
