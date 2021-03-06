﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamrealm.Base
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
    }

    public static class ColorExtensions
    {
        public static Color Lerp(this Color from, Color to, float amount)
        {
            double sr = from.R, sg = from.G, sb = from.B;

            double er = to.R, eg = to.G, eb = to.B;

            var r = sr.Lerp(er, amount);
            var g = sg.Lerp(eg, amount);
            var b = sb.Lerp(eb, amount);

            return Color.FromRgb(r, g, b);
        }

        public static List<Color> GenerateShades(this Color color, int howMany) => Enumerable.Range(1, howMany)
                .Reverse()
                .Select(n => n * 1 / (float)howMany)
                .Select(n => color.Lerp(Color.White, n))
                .ToList();

        public static double Lerp(this double start, double end, double by)
        {
            var normalized = Math.Max(Math.Min(by, 1), 0);
            return start * normalized + end * (1 - normalized);
        }

        public static string ToHexString(this Color color) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }
            // Do your translation lookup here, using whatever method you require
            var imageSource = ImageSource.FromResource(Source);

            return imageSource;
        }
    }

    public static class PropertyChangedExtensions
    {
        public static void WhenPropertyChanged<T>(this T obj, string property,
            Action<T> action) where T : INotifyPropertyChanged
        {
            obj.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == property)
                    action((T)sender);
            };
        }

        public static void WhenPropertyChanged<T>(this T obj, string property,
            Predicate<T> predicate, Action<T> action) where T : INotifyPropertyChanged
        {
            obj.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == property && predicate((T)sender))
                    action((T)sender);
            };
        }

        public static void WhenCollectionChanged<T>(this T obj,
            Func<T, NotifyCollectionChangedEventArgs, bool> predicate, Action<T> action)
            where T : INotifyCollectionChanged
        {
            obj.CollectionChanged += (sender, e) =>
            {
                if (predicate((T)sender, e))
                    action((T)sender);
            };
        }
    }
}
