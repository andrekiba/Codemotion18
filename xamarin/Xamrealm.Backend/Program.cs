using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Realms;
using Realms.Server;
using Realms.Sync;

namespace Xamrealm.Backend
{
    internal class Program
    {
        private static void Main(string[] args) => AsyncContext.Run(AsyncMain);

        private static async Task AsyncMain()
        {
            try
            {
                var credentials = Credentials.UsernamePassword(Constants.RosUsername, Constants.RosPassword, false);
                var admin = await User.LoginAsync(credentials, new Uri($"http://{Constants.RosUrl}"));

                // Hack to create permissions only the first time it's used
                // this is the only scope of the Foo class               
                var syncConfig = new SyncConfiguration(admin, new Uri($"realm://{Constants.RosUrl}/{Constants.RealmName}"));
                using (var realm = Realm.GetInstance(syncConfig))
                {
                    if (!realm.All<Foo>().Any())
                    {
                        realm.Write(() => realm.Add(new Foo { Value = 6 }));
                        await realm.GetSession().WaitForUploadAsync();

                        await admin.ApplyPermissionsAsync(PermissionCondition.Default, syncConfig.ServerUri.ToString(), AccessLevel.Write);
                    }
                }

                var notifierConfig = new NotifierConfiguration(admin)
                {
                    Handlers = { new TextAnalyticsHandler() },
                    WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), Constants.NotifierDirectory)
                };

                using (var notifier = await Notifier.StartAsync(notifierConfig))
                {
                    do
                    {
                        Console.WriteLine("'exit' to quit the app.");
                    }
                    while (Console.ReadLine() != "exit");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        private class Foo : RealmObject
        {
            public int Value { get; set; }
        }
    }
}
