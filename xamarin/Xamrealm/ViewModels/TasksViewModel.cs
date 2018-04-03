using System;
using System.Linq;
using System.Windows.Input;
using Realms;
using Xamarin.Forms;
using Xamrealm.Base;
using Xamrealm.Models;
using Xamrealm.Models.DTO;

namespace Xamrealm.ViewModels
{
    public class TasksViewModel : BaseViewModel
    {
        #region Properties

        public TaskList TaskList { get; private set; }

        #endregion

        public override void Init(object initData)
        {
            var init = (TasksInitDTO) initData;
            Realm = init.Realm;
            TaskList = Realm.Find<TaskList>(init.IdTaskList);
            RaisePropertyChanged(nameof(TaskList));
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
        }

        #region Commands

        private ICommand addTaskCommand;
        public ICommand AddTaskCommand => addTaskCommand ?? (addTaskCommand = new Command(AddTask));

        private ICommand deleteTaskCommand;
        public ICommand DeleteTaskCommand => deleteTaskCommand ?? (deleteTaskCommand = new Command<Task>(DeleteTask));

        private ICommand completeTaskCommand;
        public ICommand CompleteTaskCommand => completeTaskCommand ?? (completeTaskCommand = new Command<Task>(CompleteTask));

        #endregion

        #region Methods

        private void DeleteTask(Task task)
        {
            if (task != null)
            {
                Realm.Write(() =>
                {
                    Realm.Remove(task);
                });
            }
        }

        private void CompleteTask(Task task)
        {
            if (task != null)
            {
                Realm.Write(() =>
                {
                    task.Done = !task.Done;
                    var index = task.Done ? TaskList.Tasks.Count : TaskList.Tasks.Count(t => !t.Done);

                    TaskList.Tasks.Move(task, index - 1);
                });
            }
        }

        private void AddTask()
        {
            Realm.Write(() =>
            {
                TaskList.Tasks.Insert(0, new Task());
            });
        }

        #endregion
    }
}
