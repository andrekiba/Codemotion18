using System;
using System.Globalization;
using Xamarin.Forms;
using Xamrealm.Base;

namespace Xamrealm.Converters
{
    public class ScoreToEmojiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is float floatValue))
                return new Emoji(0x1F603).ToString();

            if (floatValue < 0.25f)
                return new Emoji(0x1F625).ToString();

            if (floatValue < 0.75f)
                return new Emoji(0x1F609).ToString();

            return new Emoji(0x1F603).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
