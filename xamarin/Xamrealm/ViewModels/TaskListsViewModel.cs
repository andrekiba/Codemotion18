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
using Xamrealm.Models;
using Xamrealm.Models.DTO;
using Constants = Xamrealm.Base.Constants;

namespace Xamrealm.ViewModels
{
    public class TaskListsViewModel : BaseViewModel
    {
        #region Properties
        public IList<TaskList> TaskLists { get; set; }
        
        #endregion

        #region Lifecycle

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            Initialize();
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

        private void Initialize()
        {
            var user = User.Current;

            var uri = user.ServerUri;
            Constants.Server.SyncHost = $"{uri.Host}:{uri.Port}";            

            try
            {
                var config = new SyncConfiguration(user, Constants.Server.SyncServerUri)
                {
                    ObjectClasses = new[] { typeof(Task), typeof(TaskList), typeof(TaskListCollection) }
                };

                Realm = Realm.GetInstance(config);

                TaskListCollection parent = null;
                Realm.Write(() =>
                {
                    // Eagerly acquire write-lock to ensure we don't get into
                    // race conditions with sync writing data in the background
                    parent = Realm.Find<TaskListCollection>(0);
                    if (parent != null)
                        return;
                    parent = Realm.Add(new TaskListCollection());
                    parent.TaskLists.Add(new TaskList
                    {
                        Id = Constants.DefaultTaskListId,
                        Title = Constants.DefaultTaskListName
                    });
                });

                TaskLists = parent.TaskLists;
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
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
