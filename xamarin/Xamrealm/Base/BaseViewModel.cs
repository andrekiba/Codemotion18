using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using FreshMvvm;
using PropertyChanged;
using Realms;

namespace Xamrealm.Base
{
    public class BaseViewModel : FreshBasePageModel
    {
        public Realm Realm { get; protected set; }

        public bool IsBusy { get; set; }
        [DependsOn(nameof(IsBusy))]
        public bool IsNotBusy => !IsBusy;

        protected async Task<bool> DoFunc(Func<Task> func, Func<Exception, Task> onError = null, string loadingMessage = null)
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

        protected async Task<TResult> Prompt<TViewModel, TResult>() where TViewModel : BaseViewModel, IPromptable<TResult>
        {
            var page = FreshPageModelResolver.ResolvePageModel<TViewModel>();
            var promptable = (TViewModel)page.BindingContext;
            var tcs = new TaskCompletionSource<TResult>();
            promptable.Success = tcs.SetResult;
            promptable.Cancel = tcs.SetCanceled;
            promptable.Error = tcs.SetException;

            await CoreMethods.PushPageModel<TViewModel>(null, true);

            try
            {
                return await tcs.Task;
            }
            finally
            {
                await CoreMethods.PopPageModel(true);
            }
        }
    } 
}
