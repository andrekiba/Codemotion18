using System;
using System.Collections.Generic;
using Realms;

namespace Xamrealm.Models
{
    public class TaskList : RealmObject, ICompletable
    {
        [PrimaryKey]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Title { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public IList<Task> Tasks { get; }
    }
}
