using System.Linq;
using System.Windows.Input;
using Realms;
using Realms.Sync;
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
        public int TaskListIndex { get; private set; }
        public int TaskListCount { get; private set; }

        #endregion

        public override void Init(object initData)
        {
            var init = (TasksInitDTO) initData;
            Realm = init.Realm;
            TaskList = Realm.Find<TaskList>(init.IdTaskList);
            TaskListIndex = init.TaskListIndex;
            TaskListCount = init.TaskListCount;

            RaisePropertyChanged(nameof(TaskList));
        }

        #region Commands

        private ICommand addTaskCommand;
        public ICommand AddTaskCommand => addTaskCommand ?? (addTaskCommand = new Command(AddTask));

        private ICommand deleteTaskCommand;
        public ICommand DeleteTaskCommand => deleteTaskCommand ?? (deleteTaskCommand = new Command<Task>(DeleteTask));

        private ICommand completeTaskCommand;
        public ICommand CompleteTaskCommand => completeTaskCommand ?? (completeTaskCommand = new Command<Task>(CompleteTask));

        private ICommand voteTaskCommand;
        public ICommand VoteTaskCommand => voteTaskCommand ?? (voteTaskCommand = new Command<Task>(VoteTask, t => !t.IsCompleted));

        #endregion

        #region Methods

        private void DeleteTask(Task task)
        {
            if (task != null)
            {
                Realm.Write(() =>
                {
                    Realm.Remove(task);
                    TaskList.IsCompleted = TaskList.Tasks.Any(t => !t.IsCompleted) == false;
                });
            }
        }

        private void CompleteTask(Task task)
        {
            if (task != null)
            {
                Realm.Write(() =>
                {
                    task.IsCompleted = !task.IsCompleted;
                    var index = task.IsCompleted ? TaskList.Tasks.Count : TaskList.Tasks.Count(t => !t.IsCompleted);
                    TaskList.Tasks.Move(task, index - 1);

                    TaskList.IsCompleted = TaskList.Tasks.Any(t => !t.IsCompleted) == false;
                });
            }
        }

        private void AddTask()
        {
            Realm.Write(() =>
            {
                TaskList.Tasks.Insert(0, new Task());
                TaskList.IsCompleted = false;
            });
        }

        private void VoteTask(Task task)
        {
            if (task == null)
                return;

            var userIdentity = User.Current.Identity;

            Realm.Write(() =>
            {
                var vote = task.Votes.FirstOrDefault(v => v.Identity == userIdentity);
                if (vote == null)
                    task.Votes.Add(new Vote { Identity = userIdentity });
                else
                {
                    task.Votes.Remove(vote);
                    Realm.Remove(vote);
                }                  
            });
        }

        #endregion
    }
}
