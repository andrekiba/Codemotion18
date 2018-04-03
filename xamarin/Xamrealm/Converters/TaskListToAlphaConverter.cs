using System;
using System.Globalization;
using Xamarin.Forms;
using Xamrealm.Models;

namespace Xamrealm.Converters
{
    public class TaskListToAlphaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskList list && !list.Done && list.Tasks.Count == 0)
                return 0.9;
            
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
