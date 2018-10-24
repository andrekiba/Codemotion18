using System.Linq;
using System.Windows.Input;
using Acr.UserDialogs;
using Realms;
using Realms.Sync;
using Xamarin.Forms;
using Xamrealm.Base;
using Xamrealm.Models;
using Credentials = Realms.Sync.Credentials;
using TTask = System.Threading.Tasks.Task;
using System.Threading;
using System;

namespace Xamrealm.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Properties

        public LoginInfo LoginInfo { get; }
        public string Password { get; set; }

        #endregion

        #region Lifecycle

        public LoginViewModel()
        {
            var cacheConfig = new RealmConfiguration("logincache.realm")
            {
                ObjectClasses = new[] { typeof(LoginInfo) }
            };

            Realm = Realm.GetInstance(cacheConfig);
            var loginInfo = Realm.All<LoginInfo>().FirstOrDefault();
            if (loginInfo == null)
            {
                loginInfo = new LoginInfo
                {
                    ServerUrl = Constants.Server.RealmServerAddress
                };

                Realm.Write(() => Realm.Add(loginInfo));
            }

            LoginInfo = loginInfo;
        }

        #endregion

        #region Commands

        private ICommand loginCommand;
        public ICommand LoginCommand => loginCommand ?? (loginCommand = new Command(async () => await Login(), () => IsNotBusy));

        #endregion

        #region Methods

        private async TTask Login()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(1));

            await Do(
                func: async () =>
                {
                    Realm.Write(() =>
                    {
                        LoginInfo.ServerUrl = LoginInfo.ServerUrl.Replace("http://", string.Empty)
                            .Replace("https://", string.Empty)
                            .Replace("realm://", string.Empty)
                            .Replace("realms://", string.Empty);
                    });

                    Constants.Server.RealmServerAddress = LoginInfo.ServerUrl;

                    var credentials = Credentials.UsernamePassword(LoginInfo.Username, Password, false);
                    await User.LoginAsync(credentials, new Uri(Constants.Server.AuthServerUrl));
                                         
                    CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.MainContainer);
                },
                onError: async ex =>
                {
                    await TTask.Delay(500, cts.Token);
                    UserDialogs.Instance.Alert("Unable to login!", ex.Message);
                    LogException(ex);
                },
                loadingMessage: "Logging in...",
                token: cts.Token
            );
        }

        #endregion
    }
}
