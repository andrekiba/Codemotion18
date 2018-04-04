using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Xamrealm.Base
{
    public static class Constants
    {
        public const string DefaultTaskListName = "Default List";
        public const string DefaultTaskListId = "80EB1620-165B-4600-A1B1-D97032FDD9A0";

        public static class Server
        {
            public static string SyncHost { get; set; } = "127.0.0.1:9080";

            public static Uri SyncServerUri => new Uri($"realm://{SyncHost}/~/xamrealm");

            public static Uri AuthServerUri => new Uri($"http://{SyncHost}");
        }

        public static class Colors
        {
            public static readonly List<Color> TaskListColors = new List<Color>
            {
                Color.FromHex("#F44336"), //Red
                Color.FromHex("#E91E63"), //Pink
                Color.FromHex("#9C27B0"), //Purple
                Color.FromHex("#673AB7"), //Deep Purple
                Color.FromHex("#3F51B5"), //Indigo
                Color.FromHex("#2196F3"), //Blue
                Color.FromHex("#03A9F4"), //Light Blue
                Color.FromHex("#00BCD4"), //Cyan
                Color.FromHex("#009688"), //Teal
                Color.FromHex("#4CAF50"), //Green
                Color.FromHex("#8BC34A"), //Light Green
                Color.FromHex("#CDDC39"), //Lime
                Color.FromHex("#FFEB3B"), //Yellow
                Color.FromHex("#FFC107"), //Amber
                Color.FromHex("#FF9800"), //Orange
                Color.FromHex("#FF5722"), //Deep Orange
                Color.FromHex("#795548"), //Brown
                Color.FromHex("#9E9E9E"), //Grey
                Color.FromHex("#607D8B"), //Blue Grey
            };

            public static readonly Dictionary<int, List<Color>> TaskColors = TaskListColors.Select(x => new
                {
                    Key = TaskListColors.IndexOf(x),
                    Value = x.GenerateShades(10)
                })
                .ToDictionary(x => x.Key, y => y.Value);

            public static readonly Color CompletedColor = new Color(0.2, 0.2, 0.2);

        }        
    }
}
