﻿using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;
using Xamrealm.Models;

namespace Xamrealm.Converters
{
    public class TaskListToAlphaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskList list && !list.IsCompleted && !list.Tasks.Any())
                return 0.9;
            
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
