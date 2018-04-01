using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Acr.UserDialogs;
using Realms;
using Realms.Sync;
using Xamarin.Forms;
using Xamrealm.Base;
using Xamrealm.Models;
using Credentials = Realms.Sync.Credentials;

namespace Xamrealm.ViewModels
{
    public class LoginViewModel : BaseViewModel, IPromptable<User>
    {
        #region Properties

        public LoginInfo LoginInfo { get; }
        public string Password { get; set; }
        public Action<User> Success { get; set; }
        public Action Cancel { get; set; }
        public Action<Exception> Error { get; set; }

        #endregion

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
                    ServerUrl = Constants.Server.SyncHost
                };

                Realm.Write(() => Realm.Add(loginInfo));
            }

            LoginInfo = loginInfo;
        }

        #region Commands

        private ICommand loginCommand;
        public ICommand LoginCommand => loginCommand ?? (loginCommand = new Command(Login, () => IsNotBusy));

        #endregion

        #region Methods

        private async void Login()
        {
            await DoFunc(
            func: async () =>
            {
                Realm.Write(() =>
                {
                    LoginInfo.ServerUrl = LoginInfo.ServerUrl.Replace("http://", string.Empty)
                        .Replace("https://", string.Empty)
                        .Replace("realm://", string.Empty)
                        .Replace("realms://", string.Empty);
                });

                Constants.Server.SyncHost = LoginInfo.ServerUrl;

                var credentials = Credentials.UsernamePassword(LoginInfo.Username, Password, false);
                var user = await User.LoginAsync(credentials, Constants.Server.AuthServerUri);

                Success(user);
            },
            onError: async ex =>
            {
                await System.Threading.Tasks.Task.Delay(500);
                UserDialogs.Instance.Alert("Unable to login", ex.Message);
                LogException(ex);
            },
            loadingMessage: "Logging in...");
        }
    
        #endregion
    }
}
