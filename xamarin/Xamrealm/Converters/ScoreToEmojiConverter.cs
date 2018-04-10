using System;
using System.Globalization;
using Xamarin.Forms;

namespace Xamrealm.Converters
{
    public class ScoreToEmojiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is float floatValue))
                return "U+1F603";

            if (floatValue < 0.25f)
                return "U+1F625";

            if (floatValue < 0.75f)
                return "U+1F609";

            return "U+1F603";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
