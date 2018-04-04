using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Xamrealm.Base
{
    public static class ColorExtensions
    {
        public static Color Lerp(this Color from, Color to, float amount)
        {
            double sr = from.R, sg = from.G, sb = from.B;

            double er = to.R, eg = to.G, eb = to.B;

            var r = (int)sr.Lerp(er, amount);
            var g = (int)sg.Lerp(eg, amount);
            var b = (int)sb.Lerp(eb, amount);

            return Color.FromRgb(r, g, b);
        }

        public static List<Color> GenerateShades(this Color color)
        {
            return Enumerable.Range(1, 6).Reverse().Select(n => n * 0.10f).Select(n => color.Lerp(Color.White, n))
                .Union(new List<Color> {color})
                .Union(Enumerable.Range(1, 6).Select(n => n * 0.10f).Select(n => color.Lerp(Color.Black, n)))
                .ToList();
        }

        public static double Lerp(this double start, double end, double by)
        {
            return start * by + end * (1 - by);
        }

        public static string ToHexString(this Color color)
        {
            var red = (int)(color.R * 255);
            var green = (int)(color.G * 255);
            var blue = (int)(color.B * 255);
            var alpha = (int)(color.A * 255);
            var hex = $"#{alpha:X2}{red:X2}{green:X2}{blue:X2}";

            return hex;
        }
    }
}
