using Realms;

namespace Xamrealm.Models.DTO
{
    public class TasksInitDTO
    {
        public Realm Realm { get; set; }

        public string IdTaskList { get; set; }

        public int TaskListIndex { get; set; }
        public int TaskListCount { get; set; }
    }
}
