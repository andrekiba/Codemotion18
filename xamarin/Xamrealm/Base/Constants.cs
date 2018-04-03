using System;
using Xamarin.Forms;

namespace Xamrealm.Base
{
    public static class Constants
    {
        public const string DefaultListName = "Default Board";
        public const string DefaultListId = "80EB1620-165B-4600-A1B1-D97032FDD9A0";

        public static class Server
        {
            public static string SyncHost { get; set; } = "127.0.0.1:9080";

            public static Uri SyncServerUri => new Uri($"realm://{SyncHost}/~/xamrealm");

            public static Uri AuthServerUri => new Uri($"http://{SyncHost}");
        }

        public static class Colors
        {
            public static readonly Color[] BoardColors =
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

            public static readonly Color[] CardColors =
            {
                new Color(231 / 255.0, 167 / 255.0, 118 / 255.0),
                new Color(228 / 255.0, 125 / 255.0, 114 / 255.0),
                new Color(233 / 255.0, 099 / 255.0, 111 / 255.0),
                new Color(242 / 255.0, 081 / 255.0, 145 / 255.0),
                new Color(154 / 255.0, 080 / 255.0, 164 / 255.0),
                new Color(088 / 255.0, 086 / 255.0, 157 / 255.0),
                new Color(056 / 255.0, 071 / 255.0, 126 / 255.0),
            };

            public static readonly Color CompletedColor = new Color(51 / 255.0, 51 / 255.0, 51 / 255.0);
        }
    }
}
