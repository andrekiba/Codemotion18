using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Acr.UserDialogs;
using Realms;
using Realms.Sync;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamrealm.Base;
using Xamrealm.Helpers;
using Xamrealm.Models;
using Xamrealm.Models.DTO;
using Constants = Xamrealm.Base.Constants;
using TTask = System.Threading.Tasks.Task;

namespace Xamrealm.ViewModels
{
    public class TaskListsViewModel : BaseViewModel
    {
        #region Fields

        private bool isFirstLoading = true;

        #endregion

        #region Properties

        public IList<TaskList> TaskLists { get; set; }
        
        #endregion

        #region Lifecycle

        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            if (!isFirstLoading)
                return;

            await DoFunc(
                func: async () => await Initialize(),
                onError: async ex =>
                {
                    await TTask.Delay(500);
                    UserDialogs.Instance.Alert("Unable to connect to the remote server!", ex.Message);
                    LogException(ex);
                },
                loadingMessage: "Logging in..."
            );

            isFirstLoading = false;
        }

        #endregion

        #region Commands

        private ICommand addTaskListCommand;
        public ICommand AddTaskListCommand => addTaskListCommand ?? (addTaskListCommand = new Command(AddTaskList));

        private ICommand deleteTaskListCommand;
        public ICommand DeleteTaskListCommand => deleteTaskListCommand ?? (deleteTaskListCommand = new Command<TaskList>(DeleteTaskList));

        private ICommand completeTaskListCommand;
        public ICommand CompleteTaskListCommand => completeTaskListCommand ?? (completeTaskListCommand = new Command<TaskList>(CompleteTaskList));

        private ICommand openTaskListCommand;
        public ICommand OpenTaskListCommand => openTaskListCommand ?? (openTaskListCommand = new Command<TaskList>(OpenTaskList));

        private ICommand logoutCommand;
        public ICommand LogoutCommand => logoutCommand ?? (logoutCommand = new Command(Logout));

        #endregion

        #region Methods

        private async TTask Initialize()
        {
            var user = User.Current;
            var uri = user.ServerUri;
            Constants.Server.RealmServerAddress = $"{uri.Host}:{uri.Port}";

            //var permissions = await user.GetGrantedPermissionsAsync(Recipient.CurrentUser);
            //var sharedPermission = permissions.FirstOrDefault(p => p.UserId == user.Identity.ToString());

            //if(sharedPermission != null)
            //{
            //    var sharedRealmPath = $"realm://{Constants.Server.RealmServerAddress}/{sharedPermission.Path}";
            //    Constants.Server.RealmSyncConfig = new SyncConfiguration(user, new Uri(sharedRealmPath));
            //    return;
            //}

            var realmConfig = new SyncConfiguration(user, new Uri(Constants.Server.RealmServerUrl))
            {
                ObjectClasses = new[] { typeof(Board), typeof(TaskList), typeof(Task), typeof(Vote) }
            };

            Realm = await Realm.GetInstanceAsync(realmConfig);

            //create the board the first time for the first user
            var board = Realm.All<Board>().SingleOrDefault();

            if (board == null)
            {
                Realm.Write(() =>
                {
                    board = new Board();
                    board.TaskLists.Add(new TaskList
                    {
                        Title = Constants.DefaultTaskListName
                    });
                    Realm.Add(board);
                });
            }

            TaskLists = board.TaskLists;

            //add access to other users
            //var condition = PermissionCondition.UserId("mila");
            //await user.ApplyPermissionsAsync(condition, Constants.Server.RealmServerUrl, AccessLevel.Write);

            var invitation = await InvitationHelper.CreateInviteAsync(Constants.Server.RealmServerUrl);
        }

        private void AddTaskList()
        {
            Realm.Write(() =>
            {
                TaskLists.Insert(0, new TaskList());
            });
        }

        private void DeleteTaskList(TaskList list)
        {
            if (list != null)
            {
                Realm.Write(() =>
                {
                    list.Tasks.ForEach(t =>
                    {
                        t.Votes.ForEach(v => Realm.Remove(v));
                        Realm.Remove(t);
                    });
                    Realm.Remove(list);
                });
            }
        }

        private void CompleteTaskList(TaskList list)
        {
            if (list != null)
            {
                Realm.Write(() =>
                {
                    list.IsCompleted = !list.IsCompleted;
                    list.Tasks.ForEach(t => t.IsCompleted = list.IsCompleted);
                    var index = list.IsCompleted ? TaskLists.Count : TaskLists.Count(t => !t.IsCompleted);
                    TaskLists.Move(list, index - 1);
                });
            }
        }

        private async void OpenTaskList(TaskList list)
        {
            if (list != null)
                await DoFunc(
                func: async () => 
                {
                    await CoreMethods.PushPageModel<TasksViewModel>(new TasksInitDTO
                    {
                        Realm = Realm,
                        IdTaskList = list.Id,
                        TaskListIndex = TaskLists.IndexOf(list),
                        TaskListCount = TaskLists.Count
                    });
                });
        }

        private async void Logout()
        {
            var ok = await UserDialogs.Instance.ConfirmAsync("Do you really want to logout?", "Warning!", "OK", "Cancel");
            if (!ok)
                return;
            await User.Current.LogOutAsync();
            CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.LoginContainer);
        }

        #endregion
    }
}
