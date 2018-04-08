using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using FreshMvvm;
using PropertyChanged;
using Realms;
using System.Threading;

namespace Xamrealm.Base
{
    public class BaseViewModel : FreshBasePageModel
    {
        public Realm Realm { get; protected set; }

        public bool IsBusy { get; set; }
        [DependsOn(nameof(IsBusy))]
        public bool IsNotBusy => !IsBusy;

        protected async Task<bool> DoFunc(Func<Task> func, Func<Exception, Task> onError = null, string loadingMessage = null, CancellationToken token = default(CancellationToken))
        {
            var ok = true;
            IsBusy = true;

            if (loadingMessage != null)
                UserDialogs.Instance.ShowLoading(loadingMessage);        

            try
            {
                await func();
            }
            catch (Exception ex)
            {
                if (onError == null)
                    LogException(ex);
                else
                    await onError(ex);
                ok = false;
            }
            finally
            {
                IsBusy = false;
                if (loadingMessage != null)
                    UserDialogs.Instance.HideLoading();
            }

            return ok;
        }

        protected void LogException(Exception ex)
        {
            Console.WriteLine(ex);
        }
    } 
}
